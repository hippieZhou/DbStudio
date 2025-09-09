using DbStudio.Infrastructure.Uow;
using DbStudio.Infrastructure.Uow.Impl;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace DbStudio.Infrastructure
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services,
            IConfiguration configuration)
        {
            var liteDbFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "default.db");
            services.AddTransient<IUnitOfWorkFactory, UnitOfWorkFactory>(sp => new UnitOfWorkFactory(liteDbFile));
            return services;
        }
    }
}