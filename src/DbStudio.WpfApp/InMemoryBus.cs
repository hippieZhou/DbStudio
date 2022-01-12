using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using DbStudio.Application.Exceptions;
using DbStudio.Application.Interfaces;
using DbStudio.Application.Wrappers;
using DbStudio.WpfApp.Services;
using MediatR;
using Newtonsoft.Json;

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

        public async Task<Response<T>> SendAsync<T>(IRequest<Response<T>> request, CancellationToken cancellationToken = default)
        {
#if DEBUG
            var payload = JsonConvert.SerializeObject(
                new
                {
                    Now = DateTime.Now.ToString("yyyy-MM-dd HH:ff:ss"),
                    Body = request
                }, Formatting.Indented);
            Trace.WriteLine(payload);
#endif
            try
            {
                return await _mediator.Send(request, cancellationToken);
            }
            catch (Exception e)
            {
                var message = e is ValidationException ex ? string.Join(Environment.NewLine, ex.Errors) : e.Message;
                _dialogService.Error(message);
                return default;
            }
        }
    }
}