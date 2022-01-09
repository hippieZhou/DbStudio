using System;
using System.Threading;
using System.Threading.Tasks;
using DbStudio.Application.Wrappers;
using DbStudio.Infrastructure.Uow;
using FluentValidation;
using MediatR;

namespace DbStudio.Application.Features.Snapshot.Commands
{
    public class SnapshotDeleteCommand : IRequest<Response<int>>
    {
        public string DataSource { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public string InitialCatalog { get; set; }
        public string SnapshotName { get; set; }
    }
    public class SnapshotDeleteCommandValidator : AbstractValidator<SnapshotDeleteCommand>
    {
        public SnapshotDeleteCommandValidator()
        {
            RuleFor(x => x.DataSource).NotEmpty().WithMessage("主机名不能为空");
            RuleFor(x => x.UserId).NotEmpty().WithMessage("用户名不能为空");
            RuleFor(x => x.Password).NotEmpty().WithMessage("密码不能为空");
            RuleFor(x => x.InitialCatalog).NotEmpty().WithMessage("数据库不能为空");
            RuleFor(x => x.SnapshotName).NotEmpty().WithMessage("快照名称不能为空");
        }
    }

    public class SnapshotDeleteCommandHandler : IRequestHandler<SnapshotDeleteCommand, Response<int>>
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public SnapshotDeleteCommandHandler(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory ?? throw new ArgumentNullException(nameof(unitOfWorkFactory));
        }

        public async Task<Response<int>> Handle(SnapshotDeleteCommand request, CancellationToken cancellationToken)
        {
            var connString = _unitOfWorkFactory.BuildConnectionString(request.DataSource, request.UserId,
                request.Password, request.InitialCatalog);
            var uow = await _unitOfWorkFactory.CreateAsync(connString, cancellationToken: cancellationToken);
            var dropSql = $@"USE MASTER; DROP DATABASE [{request.SnapshotName}]";
            var result = await uow.ExecuteAsync(new DbCommandArgs { Sql = dropSql }, cancellationToken);
            return new Response<int>(result);
        }
    }
}