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
        private readonly string _liteDb;

        public UnitOfWorkFactory(string liteDb)
        {
            _liteDb = liteDb;
        }

        public string BuildConnectionString(
            [NotNull] string dataSource,
            [NotNull] string userId,
            [NotNull] string password,
            [NotNull] string initialCatalog = "master")
        {
            return new SqlConnectionStringBuilder
            {
                DataSource = dataSource,
                UserID = userId,
                Password = password,
                InitialCatalog = initialCatalog,
                PersistSecurityInfo = true,
                MultipleActiveResultSets = true
            }.ToString();
        }

        public IDapperUnitOfWork Create(
            [NotNull] string connectionString,
            bool transactional = false,
            RetryOptions options = default,
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            var conn = new SqlConnection(connectionString);
            conn.Open();
            return new DapperUnitOfWork(conn, options, transactional, isolationLevel);
        }

        public async Task<IDapperUnitOfWork> CreateAsync(
            [NotNull] string connectionString,
            bool transactional = false,
            RetryOptions options = default,
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
            CancellationToken cancellationToken = default)
        {
            var conn = new SqlConnection(connectionString);
            await conn.OpenAsync(cancellationToken);
            return new DapperUnitOfWork(conn, options, transactional, isolationLevel);
        }

        public ILiteDbUnitOfWork CreateLite(RetryOptions options = default)
        { 
            return new LiteDbUnitOfWork(_liteDb, options);
        }
    }
}