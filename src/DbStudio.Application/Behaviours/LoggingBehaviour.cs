using DbStudio.Application.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace DbStudio.Application.Behaviours
{
    public class LoggingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<LoggingBehaviour<TRequest, TResponse>> _logger;

        public LoggingBehaviour(ILogger<LoggingBehaviour<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            var uniqueId = Guid.NewGuid().ToString();
            _logger.LogInformation($"Begin Request Id:{uniqueId}, request name:{requestName}");

            var timer = new Stopwatch();
            timer.Start();
            try
            {
                return await next();
            }
            catch (Exception e)
            {
                var message = e is ValidationException ex ? string.Join(Environment.NewLine, ex.Errors) : e.Message;
                _logger.LogError(e, message);
                return (TResponse)Activator.CreateInstance(typeof(TResponse), message);
            }
            finally
            {
                timer.Stop();
                _logger.LogInformation(
                    $"Begin Request Id:{uniqueId}, request name:{requestName}, total request time:{timer.ElapsedMilliseconds} ms");
            }
        }
    }
}