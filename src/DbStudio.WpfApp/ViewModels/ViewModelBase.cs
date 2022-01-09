using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using DbStudio.Application.Interfaces;

namespace DbStudio.WpfApp.ViewModels
{
    public abstract class ViewModelBase : ObservableRecipient
    {
        private IEventBus _mediator;
        protected IEventBus Mediator => _mediator ??= Ioc.Default.GetRequiredService<IEventBus>();
    }
}