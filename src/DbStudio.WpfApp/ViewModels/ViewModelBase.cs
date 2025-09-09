using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging.Messages;
using DbStudio.Application.Interfaces;
using DbStudio.Application.Wrappers;
using DbStudio.WpfApp.Dialogs;
using DbStudio.WpfApp.Models;
using DbStudio.WpfApp.Services;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace DbStudio.WpfApp.ViewModels
{
    public abstract class ViewModelBase : ObservableRecipient
    {
        protected ViewModelBase()
        {
            IsActive = true;
        }

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

        protected async Task<Response<T>> ExecuteOnUILoadingAsync<T>(
            IRequest<Response<T>> request,
            CancellationToken cancellationToken = default)
        {
            var dlg = Dialog.Show<LoadingDialog>(MessageToken.MainWindow)
                .Initialize<LoadingDialogViewModel>(vm => { });
            var response = await Mediator.SendAsync(request, cancellationToken);
            dlg.Close();
            return response;
        }
    }

    public class CurrentConnChangedMessage : ValueChangedMessage<DbConnection>
    {
        public CurrentConnChangedMessage(DbConnection value) : base(value)
        {
        }
    }
}