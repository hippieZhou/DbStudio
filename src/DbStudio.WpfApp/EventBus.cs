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
                var response = await _mediator.Send(request);
                return response;
            }
            catch (ValidationException ex)
            {
                var message = string.Join(Environment.NewLine, ex.Errors);
                MessageBox.Error(message,"系统提示");
                return default;
            }
            catch (Exception ex)
            {
                MessageBox.Error(ex.Message, "系统提示");
                return default;
            }
        }
    }
}