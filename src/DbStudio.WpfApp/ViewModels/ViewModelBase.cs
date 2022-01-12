using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using DbStudio.Application.Interfaces;
using DbStudio.WpfApp.Models;
using DbStudio.WpfApp.Services;

namespace DbStudio.WpfApp.ViewModels
{
    public abstract class ViewModelBase : ObservableRecipient
    {
        private IEventBus _mediator;
        protected IEventBus Mediator => _mediator ??= Ioc.Default.GetRequiredService<IEventBus>();

        private IDialogService _message;
        protected IDialogService Message => _message ??= Ioc.Default.GetRequiredService<IDialogService>();

        private DbConnection _currentConn;
        public DbConnection CurrentConn
        {
            get => _currentConn;
            set => SetProperty(ref _currentConn, value);
        }
    }
}