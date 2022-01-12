using System.IO;
using System.Text;
using DbStudio.Application;
using DbStudio.Infrastructure;
using DbStudio.WpfApp.Extensions;
using Microsoft.Extensions.DependencyInjection;
using CommunityToolkit.Mvvm.DependencyInjection;
using Serilog;
using Serilog.Events;
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
                .WriteTo.Debug()
                .WriteTo.Async(c =>
                    c.File(Path.Combine("Logs", "log.txt"),
                        encoding: Encoding.UTF8,
                        rollingInterval: RollingInterval.Day,
                        restrictedToMinimumLevel: LogEventLevel.Warning))
                .CreateLogger();

            this.SetupGlobalExceptionHandle();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            Log.Information("Starting WPF host.");
            Ioc.Default.ConfigureServices(
                new ServiceCollection()
                    .AddMvvmLayer()
                    .AddApplicationLayer(default)
                    .AddInfrastructureLayer(default)
                    .AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true))
                    .BuildServiceProvider());
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Log.CloseAndFlush();
            base.OnExit(e);
        }
    }
}
