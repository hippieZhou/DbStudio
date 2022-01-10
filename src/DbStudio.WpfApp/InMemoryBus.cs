using System;
using System.Diagnostics;
using System.Threading.Tasks;
using DbStudio.Application.Exceptions;
using DbStudio.Application.Interfaces;
using DbStudio.Application.Wrappers;
using HandyControl.Controls;
using MediatR;
using Newtonsoft.Json;

namespace DbStudio.WpfApp
{
    public class InMemoryBus : IEventBus
    {
        private readonly IMediator _mediator;

        public InMemoryBus(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<Response<T>> SendAsync<T>(IRequest<Response<T>> request)
        {
#if DEBUG
            var payload = JsonConvert.SerializeObject(
                new
                {
                    Now = DateTime.Now.ToString("yyyy-MM-dd HH:ff:ss"),
                    Body = request
                });
            Trace.WriteLine(payload);
#endif
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