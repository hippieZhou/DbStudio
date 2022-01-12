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
            //var response = await Mediator.SendAsync(new DbConnectionTestCommand
            //{
            //    DataSource = "10.5.10.218",
            //    UserId = "sa",
            //    Password = "95938"
            //});
            //if (response != null)
            //{
            //    Message.Success("连接成功");
            //}

            await Task.Yield();
        }

        private IAsyncRelayCommand _addServerCommand;
        public IAsyncRelayCommand AddServerCommand => _addServerCommand ??= new AsyncRelayCommand(AddServer);

        private async Task AddServer()
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