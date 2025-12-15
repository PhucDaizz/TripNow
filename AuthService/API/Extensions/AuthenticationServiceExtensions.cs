using API;
using Application.Contracts;
using Application.DTOs.User;
using Domain.Entities;
using Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace CarbonTC.API.Extensions
{
    public static class AuthenticationServiceExtensions
    {
        public static IServiceCollection AddAuthenticationAndAuthorization(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddIdentity<ExtendedIdentityUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 6;

                options.User.RequireUniqueEmail = true;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>() 
            .AddDefaultTokenProviders()
            .AddSignInManager<SignInManager<ExtendedIdentityUser>>();

            // =========================================================================
            // SETUP AUTHENTICATION SCHEMES (Cookie, JWT, Google)
            // =========================================================================
            var secretKey = configuration["Jwt:Key"] ?? configuration["Jwt:SecretKey"];
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new InvalidOperationException("JWT Key not found in configuration!");
            }

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            var tokenExpiryMinutes = configuration.GetValue<int>("Jwt:TokenExpiryMinutes", 60);

            var issuer = configuration["Jwt:Issuer"] ?? "TravelNow";
            var audience = configuration["Jwt:Audience"] ?? "TravelNow-Users";

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.Cookie.Name = "GoogleAuthCookie";
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.SlidingExpiration = true;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                var issuer = configuration["Jwt:Issuer"];
                var audience = configuration["Jwt:Audience"];
                var allowedIssuers = configuration.GetSection("Jwt:AllowedIssuers").Get<string[]>();
                var allowedAudiences = configuration.GetSection("Jwt:AllowedAudiences").Get<string[]>();

                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = signingKey,

                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidIssuers = allowedIssuers,

                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidAudiences = allowedAudiences,

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,

                    NameClaimType = ClaimTypes.NameIdentifier, 
                    RoleClaimType = ClaimTypes.Role
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

                        if (string.IsNullOrEmpty(token) &&
                           context.Request.Query.TryGetValue("access_token", out var queryToken))
                        {
                            token = queryToken;
                        }

                        if (string.IsNullOrEmpty(token))
                        {
                            token = context.Request.Cookies["X-Access-Token"];
                        }

                        if (!string.IsNullOrEmpty(token))
                        {
                            context.Token = token;
                        }

                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                        logger.LogError(context.Exception, "JWT Authentication failed");

                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";

                        var errorMessage = context.Exception switch
                        {
                            SecurityTokenExpiredException => "Token has expired",
                            SecurityTokenInvalidSignatureException => "Invalid token signature",
                            SecurityTokenInvalidIssuerException => "Invalid token issuer",
                            SecurityTokenInvalidAudienceException => "Invalid token audience",
                            _ => "Authentication failed"
                        };

                        return context.Response.WriteAsJsonAsync(new
                        {
                            error = errorMessage,
                            code = "AUTH_FAILED"
                        });
                    },
                    OnTokenValidated = async context =>
                    {
                        // Chuyển sang dùng redis check blacklist
                        var userManager = context.HttpContext.RequestServices
                            .GetRequiredService<UserManager<ExtendedIdentityUser>>();

                        var userId = context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);

                        if (!string.IsNullOrEmpty(userId))
                        {
                            var user = await userManager.FindByIdAsync(userId);
                            if (user == null || !user.IsActive)
                            {
                                context.Fail("User not found or inactive");
                            }
                        }
                    }
                };
            }) 
            .AddGoogle("Google", options =>
            {
                options.ClientId = configuration["Google:ClientId"] ?? throw new InvalidOperationException("Google ClientId not configured");
                options.ClientSecret = configuration["Google:ClientSecret"] ?? throw new InvalidOperationException("Google ClientSecret not configured");
                options.CallbackPath = "/signin-google";
                options.SaveTokens = true;


                options.Scope.Add("profile");
                options.Scope.Add("email");
                options.Scope.Add("openid");

                options.ClaimActions.MapJsonKey(ClaimTypes.GivenName, "given_name");
                options.ClaimActions.MapJsonKey(ClaimTypes.Surname, "family_name");
                options.ClaimActions.MapJsonKey("picture", "picture");
                options.ClaimActions.MapJsonKey("locale", "locale");

                options.Events = new OAuthEvents
                {
                    OnCreatingTicket = async context =>
                    {
                        
                        var externalAuthService = context.HttpContext.RequestServices
                               .GetRequiredService<IExternalAuthService>();
                        

                        var email = context.Principal?.FindFirstValue(ClaimTypes.Email);
                        var givenName = context.Principal?.FindFirstValue(ClaimTypes.GivenName);
                        var surname = context.Principal?.FindFirstValue(ClaimTypes.Surname);
                        var providerKey = context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
                        var picture = context.Principal?.FindFirstValue("picture");

                        if (string.IsNullOrEmpty(email))
                        {
                            context.Fail("Không tìm thấy Email từ Google.");
                            return;
                        }
                        
                        var authResult = await externalAuthService.AuthenticateAsync(new ExternalAuthCommand
                        { 
                            Email = email, 
                            Provider = "Google", 
                            ProviderKey = providerKey,
                            FirstName = givenName,
                            LastName = surname,
                            AvatarUrl = picture
                        });

                        if (!authResult.IsSuccess)
                        {
                            context.Fail($"Authentication failed: {authResult.ErrorMessage}");
                            return;
                        }

                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.NameIdentifier, authResult.UserId),
                            new Claim(ClaimTypes.Email, email),
                            new Claim("AccessToken", authResult.AccessToken),
                            new Claim("RefreshToken", authResult.RefreshToken ?? string.Empty),
                            new Claim("Provider", "Google")
                        };

                        if (!string.IsNullOrEmpty(authResult.FullName))
                        {
                            claims.Add(new Claim(ClaimTypes.Name, authResult.FullName));
                        }

                        var appIdentity = new ClaimsIdentity(claims,
                            CookieAuthenticationDefaults.AuthenticationScheme);

                        context.Principal.AddIdentity(appIdentity);

                        var frontendUrl = configuration["Frontend:Url"] ?? "http://localhost:5173";
                        var redirectUrl = $"{frontendUrl}/oauth-callback?token={Uri.EscapeDataString(authResult.AccessToken)}";

                        context.Properties.RedirectUri = redirectUrl;
                    },
                    OnRemoteFailure = context =>
                    {
                        var logger = context.HttpContext.RequestServices
                            .GetRequiredService<ILogger<Program>>();

                        logger.LogError(context.Failure, "Google OAuth failed");

                        var frontendUrl = configuration["Frontend:Url"] ?? "http://localhost:5173";
                        var errorMessage = Uri.EscapeDataString(context.Failure?.Message ?? "Google authentication failed");

                        context.Response.Redirect($"{frontendUrl}/login?error={errorMessage}&provider=google");

                        return Task.CompletedTask;
                    }
                };
            });

            return services;
        }
    }
}