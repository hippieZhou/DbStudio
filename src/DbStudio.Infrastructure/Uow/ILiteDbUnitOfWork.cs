using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DbStudio.Infrastructure.Uow
{
    public interface ILiteDbUnitOfWork
    {
        IReadOnlyList<T> FindAll<T>(string tableName);
        T FindOne<T>(string tableName, Expression<Func<T, bool>> predicate);
        void Insert<T>(string tableName, T entity);
        int DeleteMany<T>(string tableName, Expression<Func<T, bool>> predicate);
    }
}