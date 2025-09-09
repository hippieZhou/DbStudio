using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DbStudio.Infrastructure.Uow.Impl
{
    public class LiteDbUnitOfWork : ILiteDbUnitOfWork
    {
        private bool _disposed;
        private readonly ILiteDatabase _dbContext;
        private readonly RetryOptions _options;

        public LiteDbUnitOfWork(
            string connectionString,
            RetryOptions options = default)
        {
            _dbContext = new LiteDatabase(connectionString);
            _options = options;
        }

        private ILiteCollection<T> GetCollection<T>(string tableName)
            => _dbContext.GetCollection<T>(tableName);

        public IReadOnlyList<T> FindAll<T>(string tableName)
            => Retry.Invoke(() => GetCollection<T>(tableName).FindAll().ToList(), _options);

        public T FindOne<T>(string tableName, Expression<Func<T, bool>> predicate)
            => Retry.Invoke(() => GetCollection<T>(tableName).FindOne(predicate), _options);

        public void Insert<T>(string tableName, T entity)
            => Retry.Invoke(() => GetCollection<T>(tableName).Insert(entity), _options);

        public int DeleteMany<T>(string tableName, Expression<Func<T, bool>> predicate)
            => Retry.Invoke(() => GetCollection<T>(tableName).DeleteMany(predicate), _options);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~LiteDbUnitOfWork()
            => Dispose(false);

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _dbContext?.Dispose();
            }

            _disposed = true;
        }
    }
}