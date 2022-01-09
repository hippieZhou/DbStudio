using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DbStudio.Infrastructure.Uow
{
    public interface IDapperUnitOfWork : IDisposable
    {
        Task<IEnumerable<T>> QueryAsync<T>(DbCommandArgs command, CancellationToken cancellationToken = default);
        Task<T> QueryFirstOrDefault<T>(DbCommandArgs command, CancellationToken cancellationToken = default);
        Task<int> ExecuteAsync(DbCommandArgs command, CancellationToken cancellationToken = default);
        void Commit();
        void Rollback();
    }
}