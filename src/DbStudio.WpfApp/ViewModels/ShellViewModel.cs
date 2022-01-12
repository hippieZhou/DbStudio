using System.Threading;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using DbStudio.WpfApp.Dialogs;
using DbStudio.WpfApp.Models;
using HandyControl.Controls;
using HandyControl.Tools.Extension;

namespace DbStudio.WpfApp.ViewModels
{
    public class ShellViewModel : ViewModelBase
    {
        private IAsyncRelayCommand _loadedCommand;
        public IAsyncRelayCommand LoadedCommand => _loadedCommand ??= new AsyncRelayCommand(LoadAsync);

        private async Task LoadAsync()
        {
            await Task.Yield();
        }

        private IAsyncRelayCommand _addServerCommand;
        public IAsyncRelayCommand AddServerCommand => _addServerCommand ??= new AsyncRelayCommand(AddServerAsync);

        private async Task AddServerAsync(CancellationToken arg)
        {
            var currentConn = await Dialog.Show<LoginDialog>(MessageToken.MainWindow)
                .Initialize<LoginDialogViewModel>(vm => { })
                .GetResultAsync<DbConnection>();

            if (currentConn == null)
                return;
            CurrentConn = currentConn;
        }
    }
}