using System;
using System.Threading.Tasks;
using DbStudio.Application.Exceptions;
using DbStudio.Application.Interfaces;
using DbStudio.Application.Wrappers;
using HandyControl.Controls;
using MediatR;

namespace DbStudio.WpfApp
{
    public class EventBus : IEventBus
    {
        private readonly IMediator _mediator;

        public EventBus(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<Response<T>> SendAsync<T>(IRequest<Response<T>> request)
        {
            try
            {
                return await _mediator.Send(request);
            }
            catch (Exception e)
            {
                var message = e is ValidationException ex ? string.Join(Environment.NewLine, ex.Errors) : e.Message;
                MessageBox.Error(message, "系统提示");
                return default;
            }
        }
    }
}