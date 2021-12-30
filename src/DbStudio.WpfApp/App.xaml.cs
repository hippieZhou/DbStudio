using CommunityToolkit.Mvvm.DependencyInjection;
using DbStudio.Application;
using DbStudio.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace DbStudio.WpfApp
{
    public partial class App : System.Windows.Application
    {
        public App()
        {
            Ioc.Default.ConfigureServices(
                new ServiceCollection()
                    .AddApplicationLayer(default)
                    .AddInfrastructureLayer(default)
                    .BuildServiceProvider());
        }
    }
}
