using System;
using System.Threading;
using System.Threading.Tasks;
using DbStudio.Application.Wrappers;
using DbStudio.Infrastructure.Uow;
using FluentValidation;
using MediatR;

namespace DbStudio.Application.Features.DataBase.Commands
{
    /// <summary>
    /// 重建索引
    /// </summary>
    public class DataBaseIndexRebuildCommand : IRequest<Response<int>>
    {
        public string DataSource { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public string InitialCatalog { get; set; }
    }

    public class DataBaseIndexRebuildCommandValidator : AbstractValidator<DataBaseIndexRebuildCommand>
    {
        public DataBaseIndexRebuildCommandValidator()
        {
            RuleFor(x => x.DataSource).NotEmpty().WithMessage("主机名不能为空");
            RuleFor(x => x.UserId).NotEmpty().WithMessage("用户名不能为空");
            RuleFor(x => x.Password).NotEmpty().WithMessage("密码不能为空");
            RuleFor(x => x.InitialCatalog).NotEmpty().WithMessage("数据库不能为空");
        }
    }

    public class DataBaseIndexRebuildCommandValidatorHandler :
        IRequestHandler<DataBaseIndexRebuildCommand, Response<int>>
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public DataBaseIndexRebuildCommandValidatorHandler(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory ?? throw new ArgumentNullException(nameof(unitOfWorkFactory));
        }
        public async Task<Response<int>> Handle(DataBaseIndexRebuildCommand request, CancellationToken cancellationToken)
        {
            var sql = @"
--声明变量
SET NOCOUNT ON;
DECLARE @tablename VARCHAR(128);
DECLARE @execstr VARCHAR(255);
DECLARE @objectid INT;
DECLARE @objectowner VARCHAR(255);
DECLARE @indexid INT;
DECLARE @frag DECIMAL;
DECLARE @indexname CHAR(255);
DECLARE @dbname sysname;
DECLARE @tableid INT;
DECLARE @tableidchar VARCHAR(255);
DECLARE @maxfrag TINYINT = 20;

--声明游标
DECLARE @rowCountMax BIGINT = 1000000000;
DECLARE tables CURSOR FOR
SELECT CONVERT(VARCHAR, so.id)
FROM sysobjects so
    INNER JOIN sysindexes si
        ON so.id = si.id
WHERE so.type = 'U'
      AND si.indid < 2
      AND si.rows > 0
      AND si.rows < @rowCountMax
      AND so.name NOT LIKE '%bak%'
      AND so.name NOT LIKE '%log%'
      AND so.name NOT LIKE '%[0-9]'
ORDER BY si.rows;

-- 创建一个临时表来存储碎片信息
CREATE TABLE #fraglist
(
    ObjectName CHAR(255),
    ObjectId INT,
    IndexName CHAR(255),
    IndexId INT,
    Lvl INT,
    CountPages INT,
    CountRows INT,
    MinRecSize INT,
    MaxRecSize INT,
    AvgRecSize INT,
    ForRecCount INT,
    Extents INT,
    ExtentSwitches INT,
    AvgFreeBytes INT,
    AvgPageDensity INT,
    ScanDensity DECIMAL,
    BestCount INT,
    ActualCount INT,
    LogicalFrag DECIMAL,
    ExtentFrag DECIMAL
);

--打开游标
OPEN tables;

-- 对数据库的所有表循环执行dbcc showcontig命令
FETCH NEXT FROM tables
INTO @tableidchar;

WHILE @@FETCH_STATUS = 0
BEGIN
    --对表的所有索引进行统计
    INSERT INTO #fraglist
    EXEC ('DBCC SHOWCONTIG (' + @tableidchar + ') WITH FAST, TABLERESULTS, ALL_INDEXES, NO_INFOMSGS');
    FETCH NEXT FROM tables
    INTO @tableidchar;
END;

-- 关闭释放游标
CLOSE tables;
DEALLOCATE tables;

-- 为了检查，报告统计结果
SELECT *
FROM #fraglist;

--第2阶段: (整理碎片) 为每一个要整理碎片的索引声明游标
DECLARE indexes CURSOR FOR
SELECT ObjectName,
       ObjectOwner = USER_NAME(so.uid),
       ObjectId,
       IndexName,
       ScanDensity
FROM #fraglist f
    JOIN sysobjects so
        ON f.ObjectId = so.id
WHERE LogicalFrag >= @maxfrag
      AND INDEXPROPERTY(ObjectId, IndexName, 'IndexDepth') > 0;

-- 输出开始时间
PRINT 'Started defragmenting indexes at ' + CONVERT(VARCHAR, GETDATE());
--打开游标
OPEN indexes;
--循环所有的索引
FETCH NEXT FROM indexes
INTO @tablename,
     @objectowner,
     @objectid,
     @indexname,
     @frag;
WHILE @@FETCH_STATUS = 0
BEGIN
    BEGIN TRY
        SET QUOTED_IDENTIFIER ON;
        SELECT @execstr
            = 'DBCC DBREINDEX (' + '''' + RTRIM(@objectowner) + '.' + RTRIM(@tablename) + '''' + ','''
              + RTRIM(@indexname) + ''') WITH NO_INFOMSGS';
        PRINT @execstr;
        EXEC (@execstr);
        SET QUOTED_IDENTIFIER OFF;
    END TRY
    BEGIN CATCH
        PRINT 'There was an error! ' + ERROR_MESSAGE();
    END CATCH;

    FETCH NEXT FROM indexes
    INTO @tablename,
         @objectowner,
         @objectid,
         @indexname,
         @frag;
END;
-- 关闭释放游标
CLOSE indexes;
DEALLOCATE indexes;

-- 报告结束时间
PRINT 'Finished defragmenting indexes at ' + CONVERT(VARCHAR, GETDATE());

-- 删除临时表
DROP TABLE #fraglist;";

            var connString = _unitOfWorkFactory.BuildConnectionString(request.DataSource, request.UserId,
                request.Password, request.InitialCatalog);
            var uow = await _unitOfWorkFactory.CreateAsync(connString, cancellationToken: cancellationToken);
            var result = await uow.ExecuteAsync(new DbCommandArgs { Sql = sql }, cancellationToken);
            return new Response<int>(result);
        }
    }
}