using Microsoft.AspNetCore.Http.Features;
using SocialService.API.Common.ExceptionHandling;
using SocialService.API.ExceptionHandling;
using System.Diagnostics;

namespace SocialService.API.Extensions
{
    public static class ExceptionHandlingExtensions
    {
        public static IServiceCollection AddCustomExceptionHandling(this IServiceCollection services)
        {
            services.AddExceptionHandler<ValidationExceptionHandler>();
            services.AddExceptionHandler<GlobalExceptionHandler>();

            services.AddProblemDetails(options =>
            {
                options.CustomizeProblemDetails = context =>
                {
                    context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
                    context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
                    Activity? activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
                    context.ProblemDetails.Extensions.TryAdd("traceId", activity?.Id);
                };
            });

            return services;
        }
    }
}
