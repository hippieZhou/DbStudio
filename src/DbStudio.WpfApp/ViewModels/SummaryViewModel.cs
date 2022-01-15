using System.Linq;
using System.Threading.Tasks;
using DbStudio.WpfApp.Models;
using DbStudio.Application.Features.DataBase.Queries;


namespace DbStudio.WpfApp.ViewModels
{
    public class SummaryViewModel : ViewModelBase
    {
        private DbSummaryInfo _summary;

        public DbSummaryInfo Summary
        {
            get => _summary ??= new();
            set => SetProperty(ref _summary, value);
        }

        protected override void OnActivated()
        {
            Messenger.Register<SummaryViewModel, CurrentConnChangedMessage, string>(this,
                nameof(ShellViewModel),
                async (vm, conn) =>
                {
                    CurrentConn = conn.Value;
                    await UpdateSummaryAsync();
                });
            base.OnActivated();
        }

        private async Task UpdateSummaryAsync()
        {
            var response = await Mediator.SendAsync(new DataBaseSummaryCommand
            {
                DataSource = CurrentConn.DataSource,
                UserId = CurrentConn.UserId,
                Password = CurrentConn.Password,
                InitialCatalog = CurrentConn.InitialCatalog
            });
            if (response.Succeeded)
            {
                Summary.DataSource = CurrentConn.DataSource;
                Summary.InitialCatalog = CurrentConn.InitialCatalog;
                Summary.Version = response.Data.Version;
                Summary.FileSize = response.Data.FileSize;
                Summary.FileName = response.Data.FileName;
                Summary.TableCount = response.Data.Tables.Count();

                OnPropertyChanged(nameof(Summary));
            }
        }
    }
}