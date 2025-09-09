using CommunityToolkit.Mvvm.DependencyInjection;
using DbStudio.WpfApp.Services;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DbStudio.WpfApp.Extensions
{
    public static class ApplicationExtensions
    {
        /// <summary>
        ///     全局异常处理（包含 Application 和 AppDomain）
        /// </summary>
        /// <param name="application"></param>
        public static System.Windows.Application SetupGlobalExceptionHandle(this System.Windows.Application application)
        {
            //调试模式下取消全局异常捕获
            if (Debugger.IsAttached)
                return application;

            var handler = new Action<string, object>((name, ex) =>
            {
                if (ex is not Exception e) return;
                var dlgService = Ioc.Default.GetRequiredService<IDialogService>();
                dlgService.Error(e.Message);
            });

            application.DispatcherUnhandledException += (sender, e) =>
            {
                handler?.Invoke("Application", e.Exception);
                e.Handled = true;
            };

            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                handler?.Invoke("AppDomain", e.ExceptionObject);
            };

            TaskScheduler.UnobservedTaskException += (sender, e) =>
            {
                handler?.Invoke("TaskScheduler", e.Exception);
                e.SetObserved();
            };

            return application;
        }
    }
}