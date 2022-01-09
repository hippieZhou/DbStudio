using DbStudio.Application.Interfaces;
using DbStudio.WpfApp.Services;
using DbStudio.WpfApp.Services.Impl;
using Microsoft.Extensions.DependencyInjection;

namespace DbStudio.WpfApp.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddMvvmLayer(this IServiceCollection service)
        {
            service.AddSingleton<IDialogService, DialogService>();
            service.AddSingleton<IEventBus, InMemoryBus>();
            return service;
        }
    }
}