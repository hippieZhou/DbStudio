using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DbStudio.Application.DTOs;
using DbStudio.Application.Wrappers;
using DbStudio.Domain.Entities;
using DbStudio.Infrastructure.Uow;
using FluentValidation;
using MapsterMapper;
using MediatR;

namespace DbStudio.Application.Features.Snapshot.Queries
{
    public class SnapshotQueryCommand: IRequest<Response<IReadOnlyList<DbSnapshotDto>>>
    {
        public string DataSource { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public string InitialCatalog { get; set; }
    }

    public class SnapshotQueryCommandValidator : AbstractValidator<SnapshotQueryCommand>
    {
        public SnapshotQueryCommandValidator()
        {
            RuleFor(x => x.DataSource).NotEmpty().WithMessage("主机名不能为空");
            RuleFor(x => x.UserId).NotEmpty().WithMessage("用户名不能为空");
            RuleFor(x => x.Password).NotEmpty().WithMessage("密码不能为空");
            RuleFor(x => x.InitialCatalog).NotEmpty().WithMessage("数据库不能为空");
        }
    }

    public class SnapshotQueryCommandHandler :
        IRequestHandler<SnapshotQueryCommand, Response<IReadOnlyList<DbSnapshotDto>>>
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IMapper _mapper;

        public SnapshotQueryCommandHandler(IUnitOfWorkFactory unitOfWorkFactory, IMapper mapper)
        {
            _unitOfWorkFactory = unitOfWorkFactory ?? throw new ArgumentNullException(nameof(unitOfWorkFactory));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Response<IReadOnlyList<DbSnapshotDto>>> Handle(SnapshotQueryCommand request,
            CancellationToken cancellationToken)
        {
            var connString = _unitOfWorkFactory.BuildConnectionString(request.DataSource, request.UserId,
                request.Password, request.InitialCatalog);
            var uow = await _unitOfWorkFactory.CreateAsync(connString, cancellationToken: cancellationToken);
            var sql = $@"
SELECT snapshots.name AS Name,
       snapshots.create_date AS CreatedDate
FROM sys.databases AS db
    LEFT JOIN sys.databases AS snapshots
        ON snapshots.source_database_id = db.database_id
WHERE db.name = '{request.InitialCatalog}'
      AND snapshots.source_database_id IS NOT NULL
ORDER BY CreatedDate DESC;";
            var entities =
                await uow.QueryAsync<DbSnapShot>(new DbCommandArgs { Sql = sql }, cancellationToken);
            var dtos = _mapper.Map<IEnumerable<DbSnapshotDto>>(entities);
            return new Response<IReadOnlyList<DbSnapshotDto>>(dtos.ToList());
        }
    }
}