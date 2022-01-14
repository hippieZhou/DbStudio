using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using DbStudio.WpfApp.Dialogs;
using DbStudio.WpfApp.Models;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using DbStudio.Application.Features.DbCatalog.Queries;

namespace DbStudio.WpfApp.ViewModels
{
    public class ShellViewModel : ViewModelBase
    {
        private readonly List<string> _cachedCatalogs = new();
        public ObservableCollection<string> Catalogs { get; } = new();

        private IAsyncRelayCommand _loginCommand;
        public IAsyncRelayCommand LoginCommand => _loginCommand ??= new AsyncRelayCommand(LoginAsync);

        private async Task LoginAsync(CancellationToken cancellationToken)
        {
            var currentConn = await Dialog.Show<LoginDialog>(MessageToken.MainWindow)
                .Initialize<LoginDialogViewModel>(vm => { })
                .GetResultAsync<DbConnection>();

            if (currentConn == null)
                return;
            CurrentConn = currentConn;
            RefreshCatalogCommand?.ExecuteAsync(cancellationToken);
        }

        private IRelayCommand<string> _searchCatalogCommand;

        public IRelayCommand<string> SearchCatalogCommand =>
            _searchCatalogCommand ??= new RelayCommand<string>(SearchCatalog);

        private void SearchCatalog(string args)
        {
            var items = string.IsNullOrWhiteSpace(args)
                ? _cachedCatalogs
                : _cachedCatalogs.Where(x => x.ToLower().Contains(args.Trim().ToLower()));
            Catalogs.Clear();
            foreach (var item in items)
            {
                Catalogs.Add(item);
            }
        }

        private IAsyncRelayCommand _refreshCatalogCommand;
        public IAsyncRelayCommand RefreshCatalogCommand =>
            _refreshCatalogCommand ??= new AsyncRelayCommand(RefreshCatalogAsync);

        private async Task RefreshCatalogAsync(CancellationToken cancellationToken)
        {
            var response = await Mediator.SendAsync(new DbCatalogsQueryCommand
            {
                DataSource = CurrentConn.DataSource,
                UserId = CurrentConn.UserId,
                Password = CurrentConn.Password
            }, cancellationToken);

            if (response != null)
            {
                _cachedCatalogs.Clear();
                Catalogs.Clear();
                _cachedCatalogs.AddRange(response.Data);
                foreach (var item in _cachedCatalogs)
                {
                    Catalogs.Add(item);
                }
            }
        }

        private IRelayCommand _catalogChangedCommand;

        public IRelayCommand CatalogChangedCommand =>
            _catalogChangedCommand ??= new RelayCommand(CatalogChanged);

        private void CatalogChanged()
        {
            if (string.IsNullOrWhiteSpace(CurrentConn.InitialCatalog))
            {
                CurrentConn.InitialCatalog = "master";
            }
            OnPropertyChanged(nameof(CurrentConn));
            Messenger.Send(new CurrentConnChangedMessage(CurrentConn), nameof(ShellViewModel));
        }
    }
}