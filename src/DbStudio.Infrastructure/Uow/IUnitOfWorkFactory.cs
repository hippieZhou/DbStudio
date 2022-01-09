using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace DbStudio.Infrastructure.Uow
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork Create(
            [NotNull] SqlConnectionStringBuilder connectionStringBuilder,
            bool transactional = false,
            RetryOptions options = default,
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);

        Task<IUnitOfWork> CreateAsync(
            [NotNull] SqlConnectionStringBuilder connectionStringBuilder,
            bool transactional = false,
            RetryOptions options = default,
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
            CancellationToken cancellationToken = default);
    }
}