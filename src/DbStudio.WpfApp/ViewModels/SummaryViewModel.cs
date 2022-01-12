using DbStudio.WpfApp.Models;

namespace DbStudio.WpfApp.ViewModels
{
    public class SummaryViewModel : ViewModelBase
    {
        protected override void OnActivated()
        {
            Messenger.Register<SummaryViewModel, DbConnection, string>(this, nameof(ShellViewModel), (vm, conn) =>
            {
                CurrentConn = conn;
            });
            base.OnActivated();
        }
    }
}