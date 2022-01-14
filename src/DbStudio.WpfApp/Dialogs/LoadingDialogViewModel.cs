using System;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using DbStudio.WpfApp.ViewModels;
using HandyControl.Tools.Extension;

namespace DbStudio.WpfApp.Dialogs
{
    public class LoadingDialogViewModel : ViewModelBase, IDialogResultable<bool>
    {
        public bool Result { get; set; }
        public Action CloseAction { get; set; }

        private IAsyncRelayCommand _loadedCommand;
        public IAsyncRelayCommand LoadedCommand => _loadedCommand ??= new AsyncRelayCommand(LoadedAsync);

        private async Task LoadedAsync(CancellationToken cancellationToken)
        {
            await Task.Yield();
        }
    }
}