using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using DbStudio.Application.Features.Snapshot.Queries;
using DbStudio.WpfApp.Models;

namespace DbStudio.WpfApp.ViewModels
{
    public class SnapshotViewModel : ViewModelBase
    {
        private string _snapshotName;

        public string SnapshotName
        {
            get => _snapshotName;
            set => SetProperty(ref _snapshotName, value);
        }

        private ObservableCollection<DbSnapshot> _snapshotList;

        public ObservableCollection<DbSnapshot> SnapshotList
        {
            get => _snapshotList ??= new ObservableCollection<DbSnapshot>();
            set => SetProperty(ref _snapshotList, value);
        }


        protected override void OnActivated()
        {
            Messenger.Register<SnapshotViewModel, CurrentConnChangedMessage, string>(this, nameof(ShellViewModel),
                async (vm, conn) =>
                {
                    CurrentConn = conn.Value;
                    await UpdateSnapShotListAsync();
                });
            base.OnActivated();
        }

        private async Task UpdateSnapShotListAsync()
        {
            var response = await Mediator.SendAsync(new SnapshotQueryCommand
            {
                DataSource = CurrentConn.DataSource,
                UserId = CurrentConn.UserId,
                Password = CurrentConn.Password,
                InitialCatalog = CurrentConn.InitialCatalog
            });
            if (response != null)
            {
                foreach (var item in response.Data)
                {
                    SnapshotList.Add(new DbSnapshot
                    {
                        Name = item.Name,
                        CreatedDate = item.CreatedDate
                    });
                }
            }
        }

        private IAsyncRelayCommand<string> _createSnapshotCommand;

        public IAsyncRelayCommand<string> CreateSnapshotCommand =>
            _createSnapshotCommand ??= new AsyncRelayCommand<string>(CreateSnapshotAsync);

        private Task CreateSnapshotAsync(string args, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        private IAsyncRelayCommand<DbSnapshot> _deleteSnapshotCommand;

        public IAsyncRelayCommand<DbSnapshot> DeleteSnapshotCommand =>
            _deleteSnapshotCommand ??= new AsyncRelayCommand<DbSnapshot>(DeleteSnapshotAsync);

        private Task DeleteSnapshotAsync(DbSnapshot args, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }


        private IAsyncRelayCommand<DbSnapshot> _restoreSnapshotCommand;

        public IAsyncRelayCommand<DbSnapshot> RestoreSnapshotCommand =>
            _restoreSnapshotCommand ??= new AsyncRelayCommand<DbSnapshot>(RestoreSnapshotAsync);

        private Task RestoreSnapshotAsync(DbSnapshot args, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}