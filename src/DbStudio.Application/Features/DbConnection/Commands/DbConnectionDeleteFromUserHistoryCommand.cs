using System;
using DbStudio.Application.Wrappers;
using FluentValidation;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using DbStudio.Infrastructure.Uow;
using DbStudio.Domain.Entities;

namespace DbStudio.Application.Features.DbConnection.Commands
{
    public class DbConnectionDeleteFromUserHistoryCommand: IRequest<Response<int>>
    {
        public string DataSource { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
    }

    public class DbConnectionDeleteFromUserHistoryCommandValidator :
        AbstractValidator<DbConnectionDeleteFromUserHistoryCommand>
    {
        public DbConnectionDeleteFromUserHistoryCommandValidator()
        {
            RuleFor(x => x.DataSource).NotEmpty().WithMessage("主机名不能为空");
            RuleFor(x => x.UserId).NotEmpty().WithMessage("用户名不能为空");
            RuleFor(x => x.Password).NotEmpty().WithMessage("密码不能为空");
        }
    }

    public class DbConnectionDeleteFromUserHistoryCommandHandler :
        IRequestHandler<DbConnectionDeleteFromUserHistoryCommand, Response<int>>
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public DbConnectionDeleteFromUserHistoryCommandHandler(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory ?? throw new ArgumentNullException(nameof(unitOfWorkFactory));
        }

        public Task<Response<int>> Handle(DbConnectionDeleteFromUserHistoryCommand request,
            CancellationToken cancellationToken)
        { 
            var uow = _unitOfWorkFactory.CreateLite();
            var result = uow.DeleteMany<UserDbConnection>(
                nameof(UserDbConnection),
                x => x.DataSource == request.DataSource &&
                     x.UserId == request.UserId &&
                     x.Password == request.Password);
            return Task.FromResult(new Response<int>(result));
        }
    }
}