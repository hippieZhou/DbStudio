using System;
using System.Threading;
using System.Threading.Tasks;
using DbStudio.Application.Wrappers;
using DbStudio.Domain.Entities;
using DbStudio.Infrastructure.Uow;
using FluentValidation;
using MediatR;

namespace DbStudio.Application.Features.DbConnection.Commands
{
    public class DbConnectionSaveToUserHistoryCommand : IRequest<Response<int>>
    {
        public string DataSource { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
    }

    public class DbConnectionSaveToUserHistoryCommandValidator :
        AbstractValidator<DbConnectionSaveToUserHistoryCommand>
    {
        public DbConnectionSaveToUserHistoryCommandValidator()
        {
            RuleFor(x => x.DataSource).NotEmpty().WithMessage("主机名不能为空");
            RuleFor(x => x.UserId).NotEmpty().WithMessage("用户名不能为空");
            RuleFor(x => x.Password).NotEmpty().WithMessage("密码不能为空");
        }
    }

    public class DbConnectionSaveToUserHistoryCommandHandler :
        IRequestHandler<DbConnectionSaveToUserHistoryCommand, Response<int>>
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public DbConnectionSaveToUserHistoryCommandHandler(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory ?? throw new ArgumentNullException(nameof(unitOfWorkFactory));
        }

        public Task<Response<int>> Handle(
            DbConnectionSaveToUserHistoryCommand request,
            CancellationToken cancellationToken)
        {
            using var uow = _unitOfWorkFactory.CreateLite();
            var entity =
                uow.FindOne<UserDbConnection>(nameof(UserDbConnection), x => x.DataSource == request.DataSource);
            if (entity == null)
            {
                uow.Insert(nameof(UserDbConnection), new UserDbConnection
                {
                    DataSource = request.DataSource,
                    UserId = request.UserId,
                    Password = request.Password
                });
            }

            return Task.FromResult(new Response<int>(1));
        }
    }
}