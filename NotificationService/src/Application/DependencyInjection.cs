using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.Application.Common.Behaviours;
using System.Reflection;

namespace NotificationService.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}
