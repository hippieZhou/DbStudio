﻿using System;
using System.Threading;
using System.Threading.Tasks;
using DbStudio.Application.Exceptions;
using DbStudio.Application.Interfaces;
using DbStudio.Application.Wrappers;
using DbStudio.WpfApp.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DbStudio.WpfApp
{
    public class InMemoryBus : IEventBus
    {
        private readonly IDialogService _dialogService;
        private readonly ILogger<InMemoryBus> _logger;
        private readonly IMediator _mediator;

        public InMemoryBus(IMediator mediator, IDialogService dialogService, ILogger<InMemoryBus> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Response<T>> SendAsync<T>(
            IRequest<Response<T>> request,
            CancellationToken cancellationToken = default, 
            bool throwEx = true)
        {
#if DEBUG
            var payload = JsonConvert.SerializeObject(
                new
                {
                    Now = DateTime.Now.ToString("yyyy-MM-dd HH:ff:ss"),
                    Body = request
                }, Formatting.Indented);
            _logger.LogInformation(payload);
#endif
            try
            {
                return await _mediator.Send(request, cancellationToken);
            }
            catch (Exception e)
            {
                var message = e is ValidationException ex ? string.Join(Environment.NewLine, ex.Errors) : e.Message;
                _logger.LogError(e, message);
                if (throwEx)
                {
                    _dialogService.Error(message);
                }

                return default;
            }
        }
    }
}