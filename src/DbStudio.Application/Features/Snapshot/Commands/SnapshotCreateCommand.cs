using DbStudio.Application.Wrappers;
using DbStudio.Domain.Entities;
using DbStudio.Infrastructure.Uow;
using FluentValidation;
using MediatR;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DbStudio.Application.Features.Snapshot.Commands
{
    public class SnapshotCreateCommand : IRequest<Response<int>>
    {
        public string DataSource { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public string InitialCatalog { get; set; }
        public string SnapshotName { get; set; }
    }

    public class SnapshotCreateCommandValidator : AbstractValidator<SnapshotCreateCommand>
    {
        public SnapshotCreateCommandValidator()
        {
            RuleFor(x => x.DataSource).NotEmpty().WithMessage("主机名不能为空");
            RuleFor(x => x.UserId).NotEmpty().WithMessage("用户名不能为空");
            RuleFor(x => x.Password).NotEmpty().WithMessage("密码不能为空");
            RuleFor(x => x.InitialCatalog).NotEmpty().WithMessage("数据库不能为空");
            RuleFor(x => x.SnapshotName).NotEmpty().WithMessage("快照名称不能为空");
        }
    }

    public class SnapshotCreateCommandHandler : IRequestHandler<SnapshotCreateCommand, Response<int>>
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public SnapshotCreateCommandHandler(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory ?? throw new ArgumentNullException(nameof(unitOfWorkFactory));
        }

        public async Task<Response<int>> Handle(SnapshotCreateCommand request, CancellationToken cancellationToken)
        {
            var connString = _unitOfWorkFactory.BuildConnectionString(request.DataSource, request.UserId,
                request.Password, request.InitialCatalog);
            var uow = await _unitOfWorkFactory.CreateAsync(connString, cancellationToken: cancellationToken);


            var snapshotName = $"{request.InitialCatalog}_{request.SnapshotName}_{DateTime.Now:yyyyMMddHHmmssffff}";

            var ssQuery = $@"
SELECT ss.name AS Name,
       ss.create_date AS CreatedDate
FROM sys.databases AS db
    LEFT JOIN sys.databases AS ss
        ON ss.source_database_id = db.database_id
WHERE db.name = '{request.InitialCatalog}'
      AND ss.source_database_id IS NOT NULL
      AND ss.name = '{snapshotName}';";

            var ssResult =
                await uow.QueryFirstOrDefaultAsync<DbSnapShot>(new DbCommandArgs { Sql = ssQuery }, cancellationToken);
            if (ssResult != null)
                return new Response<int>("当前快照名称已存在");

            var dfQuerySql = $@"
SELECT df.name Name,
       df.physical_name PhysicalName
FROM [{request.InitialCatalog}].sys.database_files AS df
WHERE df.type = 0;";

            var dfResult =
                await uow.QueryFirstOrDefaultAsync<DbFile>(new DbCommandArgs { Sql = dfQuerySql }, cancellationToken);
            if (dfResult == null)
                return new Response<int>("当前数据库逻辑文件不已存在");

            var savePath = Path.Combine(Path.GetDirectoryName(dfResult.PhysicalName) ?? string.Empty,
                $"{snapshotName}.mdf");
            var createSql = $@"
USE master;
CREATE DATABASE [{snapshotName}]
ON
    (
        NAME = N'{dfResult.Name}',
        FILENAME = N'{savePath}'
    ) AS SNAPSHOT OF [{request.InitialCatalog}];";


            var result = await uow.ExecuteAsync(new DbCommandArgs { Sql = createSql }, cancellationToken);

            return new Response<int>(result);
        }
    }
}