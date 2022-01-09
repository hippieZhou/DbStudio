using CommunityToolkit.Mvvm.Input;
using DbStudio.Application.Features;
using DbStudio.Application.Features.DbConnection;
using DbStudio.Application.Features.DbConnection.Commands;
using DbStudio.Application.Features.DbConnection.Queries;

namespace DbStudio.WpfApp.ViewModels
{
    public class ShellViewModel: ViewModelBase
    {
        public ShellViewModel()
        {
            
        }

        private IAsyncRelayCommand _loadedCommand;
        public IAsyncRelayCommand LoadedCommand
        {
            get
            {
                return _loadedCommand ??= new AsyncRelayCommand(async () =>
                {
                    var response = await Mediator.SendAsync(new DbConnectionTestCommand
                    {
                        DataSource = "10.5.10.218",
                         UserId = "sa",
                         Password = "95938"
                    });
                    if (response != null)
                    {
                        Message.Success("连接成功");
                    }
                });
            }
        }

    }
}