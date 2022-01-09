using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace DbStudio.Infrastructure.Uow
{
    public sealed class Retry
    {
        private static Dictionary<int, string> _transientErrors = new();
        public static T Invoke<T>(Func<T> action, RetryOptions options)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (options == null)
            {
                return action();
            }

            var counter = 1;
            while (counter <= options.MaxRetries)
            {
                try
                {
                    return action();
                }
                catch (Exception ex)
                {
                    HandleException(ex, counter, options).GetAwaiter().GetResult();
                }
                counter++;
            }

            return default;
        }

        public static void Invoke(Action action, RetryOptions options)
        {
            Invoke(() =>
            {
                action();
                return true;
            }, options);
        }

        public static async Task<T> InvokeAsync<T>(Func<Task<T>> action, RetryOptions options)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (options == null)
            {
                return await action();
            }

            var counter = 1;
            while (counter <= options.MaxRetries)
            {
                try
                {
                    return await action();
                }
                catch (Exception ex)
                {
                    await HandleException(ex, counter, options);
                }
                counter++;
            }

            return default;
        }

        public static async Task InvokeAsync(Func<Task> action, RetryOptions options)
        {
            await InvokeAsync(async () =>
            {
                await action();
                return true;
            }, options);
        }

        private static Task HandleException(Exception exception, int counter, RetryOptions options)
        {
            if (!IsTransient(exception) || counter >= options.MaxRetries)
            {
                throw exception;
            }

            var sleepTime = TimeSpan.FromMilliseconds(Math.Pow(options.WaitMillis, counter));
            return Task.Delay(sleepTime);
        }

        private static bool IsTransient(Exception exception)
        {
            return exception is SqlException sqlException
                ? _transientErrors.ContainsKey(sqlException.Number)
                : exception is TimeoutException;
        }

        public static void AddError(int code, string name)
        {
            _transientErrors.Add(code, name);
        }

        public static bool RemoveError(int code)
        {
            return _transientErrors.Remove(code);
        }

        public static void SetSource(Dictionary<int, string> source)
        {
            _transientErrors = source ?? throw new ArgumentNullException(nameof(source));
        }
    }
}