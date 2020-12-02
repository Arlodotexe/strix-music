using System;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace OwlCore.Extensions
{
    public static partial class AsyncExtensions
    {
        /// <summary>
        /// Waits for an event to fire.
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        public static async Task TryOrSkip<TException>(this Task task)
            where TException : Exception
        {
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                // ignored
            }
        }

        /// <summary>
        /// Waits for an event to fire.
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        public static async Task<TResult> TryOrSkip<TException, TResult>(this Task<TResult> task)
            where TException : Exception
        {
            var taskResult = default(TResult);

            try
            {
                taskResult = await task;
            }
            catch (Exception ex)
            {
                if (ex.GetType() == typeof(TException))
                    return taskResult;

                throw;
            }

            return taskResult;
        }

        /// <summary>
        /// Waits for an event to fire.
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        public static async Task<TResult> TryOrSkip<TResult>(this Task<TResult> task)
        {
            var taskResult = default(TResult);

            try
            {
                taskResult = await task;
            }
            catch (Exception ex)
            {
                return taskResult;
            }

            return taskResult;
        }
    }