using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
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
            Messenger.Register<SnapshotViewModel, DbConnection, string>(this, nameof(ShellViewModel),
                async (vm, conn) =>
                {
                    CurrentConn = conn;
                    await UpdateSnapShotListAsync();
                });
            base.OnActivated();
        }

        private async Task UpdateSnapShotListAsync()
        {
            await Task.Yield();
        }

        private IAsyncRelayCommand<string> _createCommand;

        public IAsyncRelayCommand<string> CreateCommand =>
            _createCommand ??= new AsyncRelayCommand<string>(CreateAsync);

        private Task CreateAsync(string args, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}