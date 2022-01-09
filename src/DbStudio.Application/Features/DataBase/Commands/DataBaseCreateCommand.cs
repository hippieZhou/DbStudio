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
    public class DataBaseCreateCommand : IRequest<Response<int>>
    {
        public string DataSource { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public string InitialEmptyDbDirectory { get; set; }
        public string EmptyDbName { get; set; }
        public string EmptyLogName { get; set; }
    }

    public class DataBaseCreateCommandValidator : AbstractValidator<DataBaseCreateCommand>
    {
        public DataBaseCreateCommandValidator()
        {
            RuleFor(x => x.DataSource).NotEmpty().WithMessage("主机名不能为空");
            RuleFor(x => x.UserId).NotEmpty().WithMessage("用户名不能为空");
            RuleFor(x => x.Password).NotEmpty().WithMessage("密码不能为空");
            RuleFor(x => x.InitialEmptyDbDirectory).NotEmpty().WithMessage("备份文件不能为空")
                .Must(Directory.Exists).WithMessage("初始化目标不存在");
            RuleFor(x => x.EmptyDbName).NotEmpty().WithMessage("数据文件名称不能为空")
                .NotEqual(x => x.EmptyLogName).WithMessage("数据文件和日志文件名称不能重复");
            RuleFor(x => x.EmptyLogName).NotEmpty().WithMessage("日志文件名称不能为空")
                .NotEqual(x => x.EmptyDbName).WithMessage("数据文件和日志文件名称不能重复");
        }
    }

    public class DataBaseCreateCommandHandler : IRequestHandler<DataBaseCreateCommand, Response<int>>
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public DataBaseCreateCommandHandler(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory ?? throw new ArgumentNullException(nameof(unitOfWorkFactory));
        }

        public async Task<Response<int>> Handle(DataBaseCreateCommand request, CancellationToken cancellationToken)
        {
            var sql = $@"
USE master;  
CREATE DATABASE {request.EmptyDbName}  
ON   
( NAME = {request.EmptyDbName},  
    FILENAME = '{Path.Combine(request.InitialEmptyDbDirectory, request.EmptyDbName)}.mdf',  
    SIZE = 10,  
    MAXSIZE = 50,  
    FILEGROWTH = 5 )  
LOG ON  
( NAME = {request.EmptyLogName}_log,  
    FILENAME = '{Path.Combine(request.InitialEmptyDbDirectory, request.EmptyLogName)}.ldf',  
    SIZE = 5MB,  
    MAXSIZE = 25MB,  
    FILEGROWTH = 5MB );  
";

            var connString =
                _unitOfWorkFactory.BuildConnectionString(request.DataSource, request.UserId, request.Password);
            var uow = await _unitOfWorkFactory.CreateAsync(connString, cancellationToken: cancellationToken);
            var result = await uow.ExecuteAsync(new DbCommandArgs { Sql = sql }, cancellationToken);
            return new Response<int>(result);
        }
    }
}