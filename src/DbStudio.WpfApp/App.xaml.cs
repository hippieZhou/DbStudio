using DbStudio.Application;
using DbStudio.Infrastructure;
using DbStudio.WpfApp.Extensions;
using Microsoft.Extensions.DependencyInjection;
using CommunityToolkit.Mvvm.DependencyInjection;

namespace DbStudio.WpfApp
{
    public partial class App
    {
        /// <summary>
        /// WpfAnalyzers
        /// </summary>
        public App()
        {
            this.SetupGlobalExceptionHandle();

            Ioc.Default.ConfigureServices(
                new ServiceCollection()
                    .AddMvvmLayer()
                    .AddApplicationLayer(default)
                    .AddInfrastructureLayer(default)
                    .BuildServiceProvider());
        }
    }
}
