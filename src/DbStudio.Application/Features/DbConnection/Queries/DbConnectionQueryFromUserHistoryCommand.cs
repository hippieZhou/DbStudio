using DbStudio.Application.DTOs;
using DbStudio.Application.Wrappers;
using DbStudio.Domain.Entities;
using DbStudio.Infrastructure.Uow;
using MapsterMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DbStudio.Application.Features.DbConnection.Queries
{
    public class DbConnectionQueryFromUserHistoryCommand :
        IRequest<Response<IReadOnlyList<UserDbConnectionDto>>>
    {

    }

    public class DbConnectionQueryFromUserHistoryCommandHandler : IRequestHandler<
        DbConnectionQueryFromUserHistoryCommand, Response<IReadOnlyList<UserDbConnectionDto>>>
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IMapper _mapper;

        public DbConnectionQueryFromUserHistoryCommandHandler(IUnitOfWorkFactory unitOfWorkFactory, IMapper mapper)
        {
            _unitOfWorkFactory = unitOfWorkFactory ?? throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public Task<Response<IReadOnlyList<UserDbConnectionDto>>> Handle(
            DbConnectionQueryFromUserHistoryCommand request, CancellationToken cancellationToken)
        {
            var uow = _unitOfWorkFactory.CreateLite();
            var entities = uow.FindAll<UserDbConnection>(nameof(UserDbConnection));
            var dtos = _mapper.Map<IEnumerable<UserDbConnectionDto>>(entities);
            return Task.FromResult(new Response<IReadOnlyList<UserDbConnectionDto>>(dtos.ToList()));
        }
    }
}