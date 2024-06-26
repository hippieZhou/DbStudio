﻿using System;
using System.Data;
using System.Threading;
using Dapper;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace DbStudio.Infrastructure.Uow.Impl
{
    public class DapperUnitOfWork : IDapperUnitOfWork
    {
        private bool _disposed;
        private IDbConnection _connection;
        private IDbTransaction _transaction;
        private readonly RetryOptions _options;

        internal DapperUnitOfWork(
            IDbConnection connection,
            RetryOptions options = default,
            bool transactional = false,
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _options = options;
            if (transactional)
            {
                _transaction = connection.BeginTransaction(isolationLevel);
            }
        }

        private CommandDefinition CreateCommandDefinition(DbCommandArgs command, CancellationToken cancellationToken)
        {
            var commandDefinition = new CommandDefinition(
                commandText: command.Sql,
                parameters: command.Param,
                commandTimeout: command.CommandTimeout,
                transaction: _transaction,
                commandType: CommandType.Text,
                cancellationToken: cancellationToken);
            return commandDefinition;
        }



        public Task<IEnumerable<T>> QueryAsync<T>(DbCommandArgs command,
            CancellationToken cancellationToken = default)
        {
            return Retry.Invoke(() => _connection.QueryAsync<T>(
                    CreateCommandDefinition(command, cancellationToken)),
                _options);
        }

        public Task<T> QueryFirstOrDefaultAsync<T>(DbCommandArgs command, CancellationToken cancellationToken = default)
        {
            return Retry.Invoke(() => _connection.QueryFirstOrDefaultAsync<T>(
                    CreateCommandDefinition(command, cancellationToken)),
                _options);
        }

        public Task<int> ExecuteAsync(DbCommandArgs command, CancellationToken cancellationToken = default)
        {
            if (command.RequiresTransaction && _transaction == null)
            {
                throw new Exception($"The command {command.GetType()} requires a transaction");
            }

            return Retry.Invoke(() => _connection.ExecuteAsync(
                    CreateCommandDefinition(command, cancellationToken)),
                _options);
        }

        public void Commit()
            => _transaction?.Commit();

        public void Rollback()
            => _transaction?.Rollback();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~DapperUnitOfWork()
            => Dispose(false);

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _transaction?.Dispose();
                _connection?.Dispose();
            }

            _transaction = null;
            _connection = null;

            _disposed = true;
        }
    }
}
