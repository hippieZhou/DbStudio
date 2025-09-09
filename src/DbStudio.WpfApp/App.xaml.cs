using CommunityToolkit.Mvvm.DependencyInjection;
using DbStudio.Application;
using DbStudio.Infrastructure;
using DbStudio.Infrastructure.Shared.Helpers;
using DbStudio.WpfApp.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using System;
using System.Windows;

namespace DbStudio.WpfApp
{
    public partial class App
    {
        /// <summary>
        /// WpfAnalyzers
        /// </summary>
        public App()
        {
            Log.Logger = new LoggerConfiguration()
#if DEBUG
                .MinimumLevel.Debug()
#else
                .MinimumLevel.Information()
#endif
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("MachineName", MachineHelper.GetMachineName())
                .Enrich.WithProperty("MacAddress", MachineHelper.GetMacAddress())
                .Enrich.WithProperty("OSVersion", MachineHelper.GetOsVersion())
                .Enrich.WithProperty("IPAddress", MachineHelper.GetIpAddress())
                .Enrich.WithProperty("UserName", MachineHelper.GetUserName())
                .WriteTo.Async(c => c.ConfigureSerilog())
                .CreateLogger();

            this.SetupGlobalExceptionHandle();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            Log.Information("Starting WPF host.");
            Ioc.Default.ConfigureServices(ConfigureServices(new ServiceCollection()));
            base.OnStartup(e);
        }

        private IServiceProvider ConfigureServices(IServiceCollection services)
        {
            return services
                .AddMvvmLayer()
                .AddApplicationLayer(default)
                .AddInfrastructureLayer(default)
                .AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true))
                .BuildServiceProvider();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Log.CloseAndFlush();
            base.OnExit(e);
        }
    }
}
