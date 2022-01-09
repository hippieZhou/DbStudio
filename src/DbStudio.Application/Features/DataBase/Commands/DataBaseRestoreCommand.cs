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
    public class DataBaseRestoreCommand: IRequest<Response<int>>
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

    public class DataBaseRestoreCommandHandler : IRequestHandler<DataBaseRestoreCommand, Response<int>>
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public DataBaseRestoreCommandHandler(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory ?? throw new ArgumentNullException(nameof(unitOfWorkFactory));
        }

        public Task<Response<int>> Handle(DataBaseRestoreCommand request, CancellationToken cancellationToken)
        {
            //todo:MyDataBaseRestoreCommandHandler
            throw new System.NotImplementedException();
        }
    }
}