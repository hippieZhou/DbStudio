using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace DbStudio.Infrastructure.Uow
{
    public interface IUnitOfWorkFactory
    {
        IDapperUnitOfWork Create(
            [NotNull] SqlConnectionStringBuilder connectionStringBuilder,
            bool transactional = false,
            RetryOptions options = default,
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);

        Task<IDapperUnitOfWork> CreateAsync(
            [NotNull] SqlConnectionStringBuilder connectionStringBuilder,
            bool transactional = false,
            RetryOptions options = default,
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
            CancellationToken cancellationToken = default);

        ILiteDbUnitOfWork Create(
            [NotNull] string connectionString,
            RetryOptions options = default);
    }
}