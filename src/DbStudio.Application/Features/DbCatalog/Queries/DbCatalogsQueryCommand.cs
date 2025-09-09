using DbStudio.Application.Wrappers;
using DbStudio.Infrastructure.Uow;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DbStudio.Application.Features.DbCatalog.Queries
{
    public class DbCatalogsQueryCommand : IRequest<Response<IReadOnlyList<string>>>
    {
        public string DataSource { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
    }

    public class DbCatalogsQueryCommandValidator : AbstractValidator<DbCatalogsQueryCommand>
    {
        public DbCatalogsQueryCommandValidator()
        {
            RuleFor(x => x.DataSource).NotEmpty().WithMessage("主机名不能为空");
            RuleFor(x => x.UserId).NotEmpty().WithMessage("用户名不能为空");
            RuleFor(x => x.Password).NotEmpty().WithMessage("密码不能为空");
        }
    }

    public class DbCatalogsQueryCommandHandler : IRequestHandler<DbCatalogsQueryCommand, Response<IReadOnlyList<string>>>
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public DbCatalogsQueryCommandHandler(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory ?? throw new ArgumentNullException(nameof(unitOfWorkFactory));
        }

        public async Task<Response<IReadOnlyList<string>>> Handle(DbCatalogsQueryCommand request,
            CancellationToken cancellationToken)
        {

            const string sql = @"
SELECT name
FROM sys.databases
WHERE source_database_id IS NULL
ORDER BY name;";

            var conn = _unitOfWorkFactory.BuildConnectionString(request.DataSource, request.UserId, request.Password);
            var uow = await _unitOfWorkFactory.CreateAsync(conn, cancellationToken: cancellationToken);
            var result = await uow.QueryAsync<string>(new DbCommandArgs { Sql = sql }, cancellationToken);
            return new Response<IReadOnlyList<string>>(result?.ToList());
        }
    }
}