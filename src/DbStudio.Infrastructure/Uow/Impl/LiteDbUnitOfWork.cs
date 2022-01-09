using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LiteDB;

namespace DbStudio.Infrastructure.Uow.Impl
{
    public class LiteDbUnitOfWork : ILiteDbUnitOfWork
    {
        private readonly LiteDatabase _dbContext;
        private readonly RetryOptions _options;

        public LiteDbUnitOfWork(
            LiteDatabase dbContext,
            RetryOptions options = default)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
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
    }
}