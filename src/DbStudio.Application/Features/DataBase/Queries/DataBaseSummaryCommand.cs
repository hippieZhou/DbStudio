using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DbStudio.Application.DTOs;
using DbStudio.Application.Wrappers;
using DbStudio.Infrastructure.Shared.Helpers;
using DbStudio.Infrastructure.Uow;
using FluentValidation;
using MediatR;

namespace DbStudio.Application.Features.DataBase.Queries
{
    public class DataBaseSummaryCommand: IRequest<Response<DataBaseSummaryDto>>
    {
        public string DataSource { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public string InitialCatalog { get; set; }
    }

    public class DataBaseSummaryCommandValidator : AbstractValidator<DataBaseSummaryCommand>
    {
        public DataBaseSummaryCommandValidator()
        {
            RuleFor(x => x.DataSource).NotEmpty().WithMessage("主机名不能为空");
            RuleFor(x => x.UserId).NotEmpty().WithMessage("用户名不能为空");
            RuleFor(x => x.Password).NotEmpty().WithMessage("密码不能为空");
            RuleFor(x => x.InitialCatalog).NotEmpty().WithMessage("数据库不能为空");
        }
    }

    public class DataBaseSummaryCommandHandler : IRequestHandler<DataBaseSummaryCommand, Response<DataBaseSummaryDto>>
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public DataBaseSummaryCommandHandler(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory ?? throw new ArgumentNullException(nameof(unitOfWorkFactory));
        }

        public async Task<Response<DataBaseSummaryDto>> Handle(DataBaseSummaryCommand request,
            CancellationToken cancellationToken)
        {
            var connString =
                _unitOfWorkFactory.BuildConnectionString(request.DataSource, request.UserId, request.Password);
            var uow = await _unitOfWorkFactory.CreateAsync(connString, cancellationToken: cancellationToken);
            var version =
                await uow.QueryFirstOrDefaultAsync<string>(
                    new DbCommandArgs { Sql = "SELECT @@VERSION" },
                    cancellationToken);

            var fileName = await uow.QueryFirstOrDefaultAsync<string>(
                new DbCommandArgs
                    { Sql = $"SELECT filename FROM master.dbo.sysdatabases WHERE name = '{request.InitialCatalog}';" },
                cancellationToken);

            var tables = await uow.QueryAsync<string>(
                new DbCommandArgs { Sql = "SELECT name FROM sysobjects WHERE xtype = 'U';" },
                cancellationToken);


            var result = new DataBaseSummaryDto
            {
                Version = version,
                FileName = fileName,
                FileSize = File.Exists(fileName) ? FileSizeFormatter.FormatSize(new FileInfo(fileName).Length) : default,
                Tables = tables
            };

            return new Response<DataBaseSummaryDto>(result);
        }
    }
}