using System.Reflection;
using DbStudio.Application.Mappings;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;

namespace DbStudio.Application.Extensions
{
    public static class MapsterExtensions
    {
        /// <summary>
        /// https://github.com/MapsterMapper/Mapster/wiki/Dependency-Injection
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static IServiceCollection AddMapster(this IServiceCollection services, Assembly assembly)
        {
            services.AddSingleton(TypeAdapterConfig.GlobalSettings);
            services.AddScoped<IMapper, Mapper>(sp => new Mapper(new MapsterProfile()));
            return services;
        }
    }
}