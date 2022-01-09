using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace DbStudio.Infrastructure.Uow.Impl
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        public IUnitOfWork Create(
            [NotNull] SqlConnectionStringBuilder connectionStringBuilder,
            bool transactional = false, 
            RetryOptions options = default, 
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            var conn = new SqlConnection(connectionStringBuilder.ToString());
            conn.Open();
            return new UnitOfWork(conn, options, transactional, isolationLevel);
        }

        public async Task<IUnitOfWork> CreateAsync(
            [NotNull] SqlConnectionStringBuilder connectionStringBuilder,
            bool transactional = false,
            RetryOptions options = default,
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
            CancellationToken cancellationToken = default)
        {
            var conn = new SqlConnection(connectionStringBuilder.ToString());
            await conn.OpenAsync(cancellationToken);
            return new UnitOfWork(conn, options, transactional, isolationLevel);
        }
    }
}