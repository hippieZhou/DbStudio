using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using LiteDB;

namespace DbStudio.Infrastructure.Uow.Impl
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        public IDapperUnitOfWork Create(
            [NotNull] SqlConnectionStringBuilder connectionStringBuilder,
            bool transactional = false, 
            RetryOptions options = default, 
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            var conn = new SqlConnection(connectionStringBuilder.ToString());
            conn.Open();
            return new DapperUnitOfWork(conn, options, transactional, isolationLevel);
        }

        public async Task<IDapperUnitOfWork> CreateAsync(
            [NotNull] SqlConnectionStringBuilder connectionStringBuilder,
            bool transactional = false,
            RetryOptions options = default,
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
            CancellationToken cancellationToken = default)
        {
            var conn = new SqlConnection(connectionStringBuilder.ToString());
            await conn.OpenAsync(cancellationToken);
            return new DapperUnitOfWork(conn, options, transactional, isolationLevel);
        }

        public ILiteDbUnitOfWork Create(
            string connectionString,
            RetryOptions options = default)
        {
            var dbContext = new LiteDatabase(connectionString);
            return new LiteDbUnitOfWork(dbContext, options);
        }
    }
}