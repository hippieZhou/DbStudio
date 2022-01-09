using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DbStudio.Infrastructure.Uow
{
    public interface IDapperUnitOfWork : IDisposable
    {
        Task<IEnumerable<T>> QueryAsync<T>(ICommand command, CancellationToken cancellationToken = default);
        Task<T> QueryFirstOrDefault<T>(ICommand command, CancellationToken cancellationToken = default);
        Task<int> ExecuteAsync(ICommand command, CancellationToken cancellationToken = default);
        void Commit();
        void Rollback();
    }
}