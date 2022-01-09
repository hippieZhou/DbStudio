using DbStudio.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DbStudio.WpfApp
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddMvvmLayer(this IServiceCollection service)
        {
            service.AddSingleton<IEventBus, EventBus>();
            return service;
        }
    }
}