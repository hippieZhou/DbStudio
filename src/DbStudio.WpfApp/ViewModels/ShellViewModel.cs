using CommunityToolkit.Mvvm.Input;
using DbStudio.Application.Features;

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

                    });
                });
            }
        }

    }
}