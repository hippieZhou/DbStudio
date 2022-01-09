using System;
using System.IO;
using DbStudio.Infrastructure.Uow;
using DbStudio.Infrastructure.Uow.Impl;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DbStudio.Infrastructure
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services, IConfiguration configuration)
        {
            var liteDbFile  = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "default.db");
            services.AddTransient<IUnitOfWorkFactory, UnitOfWorkFactory>(sp => new UnitOfWorkFactory(liteDbFile));

            //if (configuration.GetValue<bool>("UseInMemoryDatabase"))
            //{
            //    services.AddDbContext<ApplicationDbContext>(options =>
            //        options.UseInMemoryDatabase("ApplicationDb"));
            //}
            //else
            //{
            //    services.AddDbContext<ApplicationDbContext>(options =>
            //        options.UseSqlServer(
            //            configuration.GetConnectionString("DefaultConnection"),
            //            b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
            //}
            //#region Repositories
            //services.AddTransient(typeof(IGenericRepositoryAsync<>), typeof(GenericRepositoryAsync<>));
            //services.AddTransient<IProductRepositoryAsync, ProductRepositoryAsync>();
            //#endregion

            return services;
        }
    }
}