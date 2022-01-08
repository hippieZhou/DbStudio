using System.Reflection;
using DbStudio.Application.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DbStudio.Application
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddApplicationLayer(this IServiceCollection services, IConfiguration config)
        {
            services.AddMapster(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(Assembly.GetExecutingAssembly());
            //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            return services;
        }
    }
}