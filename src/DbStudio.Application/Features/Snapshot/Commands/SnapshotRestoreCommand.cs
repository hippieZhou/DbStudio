using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DbStudio.Application.Wrappers;
using DbStudio.Infrastructure.Uow;
using FluentValidation;
using MediatR;

namespace DbStudio.Application.Features.Snapshot.Commands
{
    public class SnapshotRestoreCommand: IRequest<Response<int>>
    {
        public string DataSource { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public string InitialCatalog { get; set; }
        public string SnapshotName { get; set; }
    }

    internal class SnapshotRestoreCommandValidator : AbstractValidator<SnapshotRestoreCommand>
    {
        public SnapshotRestoreCommandValidator()
        {
            RuleFor(x => x.DataSource).NotEmpty().WithMessage("主机名不能为空");
            RuleFor(x => x.UserId).NotEmpty().WithMessage("用户名不能为空");
            RuleFor(x => x.Password).NotEmpty().WithMessage("密码不能为空");
            RuleFor(x => x.InitialCatalog).NotEmpty().WithMessage("数据库不能为空");
            RuleFor(x => x.SnapshotName).NotEmpty().WithMessage("快照名称不能为空");
        }
    }

    public class SnapshotRestoreCommandHandler : IRequestHandler<SnapshotRestoreCommand, Response<int>>
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public SnapshotRestoreCommandHandler(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory ?? throw new ArgumentNullException(nameof(unitOfWorkFactory));
        }

        public async Task<Response<int>> Handle(SnapshotRestoreCommand request, CancellationToken cancellationToken)
        {
            var connString = _unitOfWorkFactory.BuildConnectionString(request.DataSource, request.UserId,
                request.Password, request.InitialCatalog);
            var uow = await _unitOfWorkFactory.CreateAsync(connString, cancellationToken: cancellationToken);

            #region 删除非目标快照

            var querySql = $@"
SELECT ss.name AS SnapshotName
FROM sys.databases AS db
    LEFT JOIN sys.databases AS ss
        ON ss.source_database_id = db.database_id
WHERE db.name = '{request.InitialCatalog}'
      AND ss.source_database_id IS NOT NULL
      AND ss.name <> '{request.SnapshotName}';";

            var ssResult = await uow.QueryAsync<string>(new DbCommandArgs { Sql = querySql }, cancellationToken);
            if (ssResult != null && ssResult.Any())
            {
                var dropSnapshotNames = string.Join("] , [",
                    ssResult.Where(x => !string.Equals(x, request.SnapshotName)));
                var dropSql = $@"USE MASTER; DROP DATABASE [{dropSnapshotNames}]";
                var rows = await uow.ExecuteAsync(new DbCommandArgs { Sql = dropSql }, cancellationToken);
            }

            #endregion

            #region 还原目标快照

            var execSql = $@"
USE master;
ALTER DATABASE [{request.InitialCatalog}]
SET SINGLE_USER
WITH ROLLBACK IMMEDIATE;
RESTORE DATABASE [{request.InitialCatalog}]
FROM DATABASE_SNAPSHOT = '{request.SnapshotName}';
ALTER DATABASE [{request.InitialCatalog}]
SET MULTI_USER
WITH ROLLBACK IMMEDIATE;";
            var result = await uow.ExecuteAsync(new DbCommandArgs { Sql = execSql }, cancellationToken);
            return new Response<int>(result);

            #endregion
        }
    }
}