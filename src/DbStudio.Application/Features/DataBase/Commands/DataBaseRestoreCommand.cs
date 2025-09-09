using DbStudio.Application.DTOs;
using DbStudio.Application.Wrappers;
using DbStudio.Domain.Entities;
using DbStudio.Infrastructure.Uow;
using FluentValidation;
using MediatR;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DbStudio.Application.Features.DataBase.Commands
{
    public class DataBaseRestoreCommand : IRequest<Response<DbBackupDto>>
    {
        public string DataSource { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public string InitialBakFile { get; set; }
        public string InitialBakDirectory { get; set; }
    }

    public class MyDataBaseRestoreCommandValidator : AbstractValidator<DataBaseRestoreCommand>
    {
        public MyDataBaseRestoreCommandValidator()
        {
            RuleFor(x => x.DataSource).NotEmpty().WithMessage("主机名不能为空");
            RuleFor(x => x.UserId).NotEmpty().WithMessage("用户名不能为空");
            RuleFor(x => x.Password).NotEmpty().WithMessage("密码不能为空");
            RuleFor(x => x.InitialBakFile).NotEmpty().WithMessage("备份文件不能为空")
                .Must(File.Exists).WithMessage("备份文件不存在");
        }
    }

    public class DataBaseRestoreCommandHandler : IRequestHandler<DataBaseRestoreCommand, Response<DbBackupDto>>
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public DataBaseRestoreCommandHandler(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory ?? throw new ArgumentNullException(nameof(unitOfWorkFactory));
        }

        public async Task<Response<DbBackupDto>> Handle(DataBaseRestoreCommand request,
            CancellationToken cancellationToken)
        {
            var dbCatalog = Path.GetFileNameWithoutExtension(request.InitialBakFile);

            var conn = _unitOfWorkFactory.BuildConnectionString(request.DataSource, request.UserId, request.Password);
            var uow = await _unitOfWorkFactory.CreateAsync(conn, cancellationToken: cancellationToken);

            //1.删除快照
            await DeleteAllSnapShotsAsync(uow, dbCatalog);

            //2.获取物理路径
            var physicalLocationResult = await GetPhysicalLocationAsync(uow, dbCatalog);

            //获取还原的数据库路径
            var dbLocation = Directory.Exists(request.InitialBakDirectory)
                ? request.InitialBakDirectory
                : (string.IsNullOrWhiteSpace(physicalLocationResult) ||
                   !Directory.Exists(physicalLocationResult)
                    ? Path.GetDirectoryName(request.InitialBakFile)
                    : physicalLocationResult);

            var dbInfoResult = await RestoreFileListOnly(uow, request.InitialBakFile);

            var _ = await RestoreDataBaseAsync(
                uow,
                dbCatalog,
                dbLocation,
                request.InitialBakFile,
                dbInfoResult.LogicalName,
                dbInfoResult.LogicalLogName);

            return new Response<DbBackupDto>(new DbBackupDto { Catalog = dbCatalog, Path = dbLocation });
        }

        /// <summary>
        /// 删除所有快照
        /// </summary>
        private async Task<int> DeleteAllSnapShotsAsync(IDapperUnitOfWork uow, string catalog)
        {
            var querySql = $@"
SELECT snapshots.name AS SnapshotName
FROM sys.databases AS db
    LEFT JOIN sys.databases AS snapshots
        ON snapshots.source_database_id = db.database_id
WHERE db.name = '{catalog}'
      AND snapshots.source_database_id IS NOT NULL;";
            var snapshots = await uow.QueryAsync<string>(new DbCommandArgs { Sql = querySql });
            var enumerable = snapshots as string[] ?? snapshots.ToArray();
            if (!enumerable.Any())
            {
                return 0;
            }

            var dropSnapshotNames = string.Join("] , [", enumerable);
            var dropSql = $@"USE MASTER; DROP DATABASE [{dropSnapshotNames}]";
            return await uow.ExecuteAsync(new DbCommandArgs { Sql = dropSql });
        }

        private async Task<string> GetPhysicalLocationAsync(IDapperUnitOfWork uow,
            string catalog)
        {
            var querySql = $@"
SELECT physical_name
FROM sys.master_files
WHERE database_id = DB_ID(N'{catalog}')
      AND type_desc = 'ROWS';";
            var result = await uow.QueryFirstOrDefaultAsync<string>(new DbCommandArgs { Sql = querySql });
            return Path.GetDirectoryName(result);
        }

        /// <summary>
        /// 获取备份库文件对应的库名称和日志库名称
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="bakFileName"></param>
        /// <returns></returns>
        private async Task<DbDbFileListOnlyDto> RestoreFileListOnly(IDapperUnitOfWork uow, string bakFileName)
        {
            var sqlRestore = $"RESTORE filelistonly FROM DISK='{bakFileName}'";
            var result = await uow.QueryAsync<DbFileListOnly>(new DbCommandArgs { Sql = sqlRestore });
            var dbFileListOnlies = result as DbFileListOnly[] ?? result.ToArray();
            if (!dbFileListOnlies.Any())
            {
                return default;
            }

            var logicalName = dbFileListOnlies.FirstOrDefault(x => string.Equals(x.Type, "D"))?.LogicalName;
            var logicalLogName = dbFileListOnlies.FirstOrDefault(x => string.Equals(x.Type, "L"))?.LogicalName;
            return new DbDbFileListOnlyDto { LogicalName = logicalName, LogicalLogName = logicalLogName };
        }

        private async Task<int> RestoreDataBaseAsync(
            IDapperUnitOfWork uow,
            string catalogName,
            string dbLocation,
            string dbBakFile,
            string logicalName,
            string logicalLogName)
        {
            var sql = string.Format(@"
USE master;
--关闭数据库的引用
IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'{0}')
    ALTER DATABASE [{0}] SET OFFLINE WITH ROLLBACK IMMEDIATE;
--如果要创建的数据库已经存在，那么删除它
IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'{0}')
    DROP DATABASE [{0}];
--创建一个新数据库，要指定新建数据库的数据文件和日志文件的名称和位置，初始化大小增长幅度，最大值等内容
CREATE DATABASE [{0}]
ON
    (
        NAME = N'{3}',
        FILENAME = N'{1}\{0}_data.mdf',
        SIZE = 5MB,
        MAXSIZE = 50MB,
        FILEGROWTH = 5MB
    )
LOG ON
    (
        NAME = N'{4}',
        FILENAME = N'{1}\{0}_log.ldf',
        SIZE = 5MB,
        MAXSIZE = 25MB,
        FILEGROWTH = 5MB
    );
--把指定的数据库备份文件恢复到刚刚建立的数据库里，这里要指定数据库备份文件的位置
--以及要恢复到的数据库，因为备份文件来自未知的机器，备份的时候原数据库和新数据库
--的数据文件和日志文件的位置不匹配，所以要用with move指令来完成强制文件移动，如果
--是通过管理器备份的数据库文件，数据库文件和日志文件名分别是数据库名跟上_Data或
--_Log，这是一个假设哦，如果不是这样，脚本有可能会出错
RESTORE DATABASE [{0}]
FROM DISK = '{2}'
WITH FILE = 1,
     MOVE '{3}'
     TO '{1}\{0}.mdf',
     MOVE '{4}'
     TO '{1}\{0}.ldf',
     NOUNLOAD,
     REPLACE,
     STATS = 10;
--打开数据库的引用
ALTER DATABASE [{0}] SET ONLINE;",
                catalogName, dbLocation, dbBakFile, logicalName, logicalLogName);

            return await uow.ExecuteAsync(new DbCommandArgs { Sql = sql });
        }
    }
}