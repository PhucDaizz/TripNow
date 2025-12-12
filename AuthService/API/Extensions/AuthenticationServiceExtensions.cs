using Application.DTOs.User;
using Application.Contracts;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Domain.Entities;

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
            })
            .AddEntityFrameworkStores<ApplicationDbContext>() 
            .AddDefaultTokenProviders();

            // =========================================================================
            // SETUP AUTHENTICATION SCHEMES (Cookie, JWT, Google)
            // =========================================================================
            var secretKey = configuration["Jwt:Key"] ?? configuration["Jwt:SecretKey"];
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new InvalidOperationException("JWT Key not found in configuration!");
            }

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
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
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,

                    ValidIssuer = issuer,
                    ValidIssuers = allowedIssuers,
                    ValidAudience = audience,
                    ValidAudiences = allowedAudiences,

                    NameClaimType = ClaimTypes.NameIdentifier, 
                    RoleClaimType = ClaimTypes.Role
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        if (context.Request.Query.TryGetValue("access_token", out var token))
                        {
                            context.Token = token;
                        }
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        return Task.CompletedTask;
                    }
                };
            }) 
            .AddGoogle("Google", options =>
            {
                options.ClientId = configuration["Google:ClientId"];
                options.ClientSecret = configuration["Google:ClientSecret"];
                options.CallbackPath = "/signin-google";
                options.SaveTokens = true;

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

                        if (string.IsNullOrEmpty(email))
                        {
                            context.Fail("Không tìm thấy Email từ Google.");
                            return;
                        }
                        
                        var authResult = await externalAuthService.AuthenticateAsync(new ExternalAuthCommand
                        { 
                            Email = email, Provider = "Google", ProviderKey = providerKey 
                        });
                        
                        if (!authResult.IsSuccess) { context.Fail("Lỗi login"); return; }
                        
                        var mySystemToken = authResult.AccessToken;
                        

                        context.Properties.RedirectUri = $"/api/auth/google-callback?token={mySystemToken}";

                        await Task.CompletedTask;
                    },
                    OnRemoteFailure = context =>
                    {
                        context.HandleResponse();
                        context.Response.Redirect("http://localhost:5173/login?error=GoogleAuthFailed");
                        return Task.CompletedTask;
                    }
                };
            });

            return services;
        }
    }
}