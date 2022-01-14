using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DbStudio.Application.Wrappers;
using DbStudio.Infrastructure.Uow;
using FluentValidation;
using MediatR;

namespace DbStudio.Application.Features.DataBase.Commands
{
    public class DataBaseBackupCommand : IRequest<Response<FileInfo>>
    {
        public string DataSource { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public string InitialCatalog { get; set; }
        public string PhysicalDirectory { get; set; }

        /// <summary>
        /// 是否启用差异备份(https://sqlbak.com/academy/database-backup)
        /// </summary>
        public bool EnableDiff { get; set; }
    }

    public class MyDataBaseBackupCommandValidator : AbstractValidator<DataBaseBackupCommand>
    {
        public MyDataBaseBackupCommandValidator()
        {
            RuleFor(x => x.DataSource).NotEmpty().WithMessage("主机名不能为空");
            RuleFor(x => x.UserId).NotEmpty().WithMessage("用户名不能为空");
            RuleFor(x => x.Password).NotEmpty().WithMessage("密码不能为空");
            RuleFor(x => x.InitialCatalog).NotEmpty().WithMessage("数据库不能为空");
            RuleFor(x => x.PhysicalDirectory).NotEmpty().WithMessage("路径不能为空")
                .Must(Directory.Exists).WithMessage("目录不存在");
        }
    }

    public class DataBaseBackupCommandHandler : IRequestHandler<DataBaseBackupCommand, Response<FileInfo>>
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public DataBaseBackupCommandHandler(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory ?? throw new ArgumentNullException(nameof(unitOfWorkFactory));
        }

        public async Task<Response<FileInfo>> Handle(DataBaseBackupCommand request, CancellationToken cancellationToken)
        {
            var connString =
                _unitOfWorkFactory.BuildConnectionString(request.DataSource, request.UserId, request.Password);
            var uow = await _unitOfWorkFactory.CreateAsync(connString, cancellationToken: cancellationToken);
            var bakFile = Path.Combine(request.PhysicalDirectory, $"{request.InitialCatalog}.bak");
            var bakSql = $"BACKUP DATABASE [{request.InitialCatalog}] TO DISK = '{bakFile}'";
            if (request.EnableDiff)
            {
                bakSql += " WITH DIFFERENTIAL";
            }

            var _ = await uow.ExecuteAsync(new DbCommandArgs { Sql = bakSql, }, cancellationToken);
            return new Response<FileInfo>(new FileInfo(bakFile));
        }
    }
}