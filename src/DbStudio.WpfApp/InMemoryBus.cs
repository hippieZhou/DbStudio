using DbStudio.Application.Interfaces;
using DbStudio.Application.Wrappers;
using DbStudio.WpfApp.Services;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DbStudio.WpfApp
{
    public class InMemoryBus : IEventBus
    {
        private readonly IDialogService _dialogService;
        private readonly IMediator _mediator;

        public InMemoryBus(IMediator mediator, IDialogService dialogService)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
        }

        public async Task<Response<T>> SendAsync<T>(
            IRequest<Response<T>> request,
            CancellationToken cancellationToken = default,
            bool throwEx = true)
        {
            var response = await _mediator.Send(request, cancellationToken);
            if (response.Succeeded == false && throwEx)
            {
                _dialogService.Error(response.Message);
            }

            return response;
        }
    }
}