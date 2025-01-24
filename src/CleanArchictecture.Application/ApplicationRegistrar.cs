using CleanArhictecture.Application.Behaviors;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchictecture.Application;

public static class ApplicationRegistrar
{
        public static IServiceCollection AddApplicaton(this IServiceCollection services)
        {
            services.AddMediatR(conf =>
            {
                conf.RegisterServicesFromAssembly(typeof(ApplicationRegistrar).Assembly);
                conf.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });

            services.AddValidatorsFromAssembly(typeof(ApplicationRegistrar).Assembly);

            return services;
        }
         
}

