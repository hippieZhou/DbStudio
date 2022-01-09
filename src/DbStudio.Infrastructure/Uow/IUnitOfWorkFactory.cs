using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace DbStudio.Infrastructure.Uow
{
    public interface IUnitOfWorkFactory
    {
        string BuildConnectionString(
            [NotNull] string dataSource,
            [NotNull] string userId,
            [NotNull] string password,
            [NotNull] string initialCatalog = "master");

        IDapperUnitOfWork Create(
            [NotNull] string connectionString,
            bool transactional = false,
            RetryOptions options = default,
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);

        Task<IDapperUnitOfWork> CreateAsync(
            [NotNull] string connectionString,
            bool transactional = false,
            RetryOptions options = default,
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
            CancellationToken cancellationToken = default);

        ILiteDbUnitOfWork Create(
            [NotNull] string connectionString,
            RetryOptions options = default);
    }
}