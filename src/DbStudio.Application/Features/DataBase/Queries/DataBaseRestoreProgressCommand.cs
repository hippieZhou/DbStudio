using DbStudio.Application.Wrappers;
using DbStudio.Domain.Entities;
using DbStudio.Infrastructure.Uow;
using FluentValidation;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DbStudio.Application.Features.DataBase.Queries
{
    public class DataBaseRestoreProgressCommand : IRequest<Response<int?>>
    {
        public string DataSource { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
    }

    public class DataBaseRestoreProgressCommandValidator : AbstractValidator<DataBaseRestoreProgressCommand>
    {
        public DataBaseRestoreProgressCommandValidator()
        {
            RuleFor(x => x.DataSource).NotEmpty().WithMessage("主机名不能为空");
            RuleFor(x => x.UserId).NotEmpty().WithMessage("用户名不能为空");
            RuleFor(x => x.Password).NotEmpty().WithMessage("密码不能为空");
        }
    }

    public class
        DataBaseRestoreProgressCommandHandler : IRequestHandler<DataBaseRestoreProgressCommand, Response<int?>>
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public DataBaseRestoreProgressCommandHandler(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory ?? throw new ArgumentNullException(nameof(unitOfWorkFactory));
        }

        public async Task<Response<int?>> Handle(DataBaseRestoreProgressCommand request,
            CancellationToken cancellationToken)
        {

            const string sqlFormat = @"
SELECT DB_NAME(er.[database_id]) [DatabaseName],
       er.[session_id] AS [SessionID],
       er.[command] AS [CommandType],
       est.[text] AS [StatementText],
       er.[status] AS [Status],
       CONVERT(INT, er.[percent_complete]) AS [CompletePercent],
       CONVERT(DECIMAL(38, 2), er.[total_elapsed_time] / 60000.00) AS [ElapsedTime],
       CONVERT(DECIMAL(38, 2), er.[estimated_completion_time] / 60000.00) AS [EstimatedCompletionTime],
       er.[last_wait_type] [LastWait],
       er.[wait_resource] [CurrentWait]
FROM sys.dm_exec_requests AS er
    INNER JOIN sys.dm_exec_sessions AS es
        ON er.[session_id] = es.[session_id]
    CROSS APPLY sys.dm_exec_sql_text(er.[sql_handle]) est
WHERE er.[command] = 'RESTORE DATABASE';";

            var connString =
                _unitOfWorkFactory.BuildConnectionString(request.DataSource, request.UserId, request.Password);
            var uow = await _unitOfWorkFactory.CreateAsync(connString, cancellationToken: cancellationToken);
            var result =
                await uow.QueryFirstOrDefaultAsync<DbExec>(new DbCommandArgs { Sql = sqlFormat }, cancellationToken);
            return new Response<int?>(result?.CompletePercent);
        }
    }
}