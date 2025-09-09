using CommunityToolkit.Mvvm.Input;
using DbStudio.Application.Features.DataBase.Queries;
using DbStudio.WpfApp.ViewModels;
using HandyControl.Tools.Extension;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DbStudio.WpfApp.Dialogs
{
    public class LoadingProgressDialogViewModel : ViewModelBase, IDialogResultable<bool>
    {
        public bool Result { get; set; }
        public Action CloseAction { get; set; }

        private int _progress;

        public int Progress
        {
            get => _progress;
            set => SetProperty(ref _progress, value);
        }

        private IAsyncRelayCommand _loadedCommand;
        public IAsyncRelayCommand LoadedCommand => _loadedCommand ??= new AsyncRelayCommand(LoadedAsync);

        private async Task LoadedAsync(CancellationToken cancellationToken)
        {
            Progress = 0;
            var request = new DataBaseRestoreProgressCommand
            {
                DataSource = CurrentConn.DataSource,
                UserId = CurrentConn.UserId,
                Password = CurrentConn.Password
            };

            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(500), cancellationToken);
                var response = await Mediator.SendAsync(request, cancellationToken);
                if (response.Succeeded)
                {
                    Progress = Math.Max(Progress, response.Data.GetValueOrDefault());
                }
            }
        }
    }
}
