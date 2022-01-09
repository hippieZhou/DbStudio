using System;
using System.Threading;
using System.Threading.Tasks;
using DbStudio.Application.Wrappers;
using DbStudio.Infrastructure.Uow;
using FluentValidation;
using MediatR;

namespace DbStudio.Application.Features.DbConnection.Queries
{
    public class DbConnectionTestCommand : IRequest<Response<int>>
    {
        public string DataSource { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
    }

    public class MyConnectionTestCommandValidator : AbstractValidator<DbConnectionTestCommand>
    {
        public MyConnectionTestCommandValidator()
        {
            RuleFor(x => x.DataSource).NotEmpty().WithMessage("主机名不能为空");
            RuleFor(x => x.UserId).NotEmpty().WithMessage("用户名不能为空");
            RuleFor(x => x.Password).NotEmpty().WithMessage("密码不能为空");
        }
    }


    public class DbConnectionTestCommandHandler : IRequestHandler<DbConnectionTestCommand, Response<int>>
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public DbConnectionTestCommandHandler(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory ?? throw new ArgumentNullException(nameof(unitOfWorkFactory));
        }

        public async Task<Response<int>> Handle(DbConnectionTestCommand request, CancellationToken cancellationToken)
        {
            var conn = _unitOfWorkFactory.BuildConnectionString(request.DataSource, request.UserId, request.Password);
            var _ = await _unitOfWorkFactory.CreateAsync(conn, cancellationToken: cancellationToken);
            return new Response<int>(1);
        }
    }
}