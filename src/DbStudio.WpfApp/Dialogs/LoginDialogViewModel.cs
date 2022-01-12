using DbStudio.WpfApp.ViewModels;
using HandyControl.Tools.Extension;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using DbStudio.Application.Features.DbConnection.Commands;
using DbStudio.Application.Features.DbConnection.Queries;
using DbStudio.WpfApp.Models;

namespace DbStudio.WpfApp.Dialogs
{
    public class LoginDialogViewModel : ViewModelBase, IDialogResultable<DbConnection>
    {
        public DbConnection Result { get; set; }
        public Action CloseAction { get; set; }

        public ObservableCollection<DbConnection> HistoryConnections { get; } = new();

        private DbConnection _newConnection ;

        public DbConnection NewConnection
        {
            get => _newConnection ??= new();
            set => SetProperty(ref _newConnection, value);
        }

        private bool _isEnabled;

        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }

        private IAsyncRelayCommand _loadedCommand;
        public IAsyncRelayCommand LoadedCommand => _loadedCommand ??= new AsyncRelayCommand(LoadAsync);

        private async Task LoadAsync(CancellationToken cancellationToken)
        {
            IsEnabled = false;

            HistoryConnections.Clear();
            var response = await Mediator.SendAsync(new DbConnectionQueryFromUserHistoryCommand(), cancellationToken);
            if (response != null)
            {
                foreach (var dto in response.Data)
                {
                    HistoryConnections.Add(new DbConnection
                    {
                        DataSource = dto.DataSource,
                        UserId = dto.UserId,
                        Password = dto.Password,
                    });
                }
            }

            IsEnabled = true;
        }

        private IAsyncRelayCommand _testCommand;
        public IAsyncRelayCommand TestCommand => _testCommand ??= new AsyncRelayCommand(TestAsync);

        private async Task TestAsync(CancellationToken cancellationToken)
        {
            IsEnabled = false;

            var response = await Mediator.SendAsync(new DbConnectionTestCommand
            {
                DataSource = NewConnection.DataSource,
                UserId = NewConnection.UserId,
                Password = NewConnection.Password
            }, cancellationToken);
            if (response != null)
                Message.Success("数据库连接成功");

            IsEnabled = true;
        }

        private IAsyncRelayCommand<DbConnection> _connectCommand;
        public IAsyncRelayCommand<DbConnection> ConnectCommand => _connectCommand ??= new AsyncRelayCommand<DbConnection>(ConnectAsync);

        private async Task ConnectAsync(DbConnection conn, CancellationToken cancellationToken)
        {
            IsEnabled = false;

            var response = await Mediator.SendAsync(new DbConnectionTestCommand
            {
                DataSource = conn.DataSource,
                UserId = conn.UserId,
                Password = conn.Password
            }, cancellationToken);

            if (response != null)
            {
                Result = conn;
                await SaveNewConnectionToHistoryAsync(conn, cancellationToken);
            }

            IsEnabled = true;
        }

        private async Task SaveNewConnectionToHistoryAsync(DbConnection conn, CancellationToken cancellationToken)
        {
            await Mediator.SendAsync(new DbConnectionSaveToUserHistoryCommand
            {
                DataSource = conn.DataSource,
                UserId = conn.UserId,
                Password = conn.Password
            }, cancellationToken);
            CloseAction?.Invoke();
        }

        

        private IAsyncRelayCommand<DbConnection> _deleteHistoryCommand;
        public IAsyncRelayCommand<DbConnection> DeleteHistoryCommand => _deleteHistoryCommand ??= new AsyncRelayCommand<DbConnection>(DeleteHistoryAsync);

        private async Task DeleteHistoryAsync(DbConnection conn, CancellationToken cancellationToken)
        {
           var response = await Mediator.SendAsync(new DbConnectionDeleteFromUserHistoryCommand
            {
                DataSource = conn.DataSource,
                UserId = conn.UserId,
                Password = conn.Password
            }, cancellationToken);

           if (response != null)
           {
               LoadedCommand?.ExecuteAsync(cancellationToken);
           }
        }

        private IRelayCommand _cancelCommand;
        public IRelayCommand CancelCommand => _cancelCommand ??= new RelayCommand(Cancel);

        private void Cancel()
        {
            Result = null;
            CloseAction?.Invoke();
        }
    }
}