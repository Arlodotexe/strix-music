using System;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace OwlCore.Extensions
{
    public static partial class AsyncExtensions
    {
        /// <summary>
        /// Syntactic sugar for catching any exception that may occur in a <see cref="Task"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        public static async Task TryOrSkip<TException>(this Task task)
            where TException : Exception
        {
            try
            {
                await task;
            }
            catch (TException)
            {
                // ignored
            }
        }

        /// <summary>
        /// Syntactic sugar for catching a specific exception that may occur in a <see cref="Task"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. Default (nullable) if the task fails.</returns>
        public static async Task<TResult> TryOrSkip<TException, TResult>(this Task<TResult> task)
            where TException : Exception
            where TResult : class
        {
            try
            {
                return await task;
            }
            catch (TException)
            {
                return default!;
            }
        }

        /// <summary>
        /// Syntactic sugar for catching any exception that may occur in a <see cref="Task"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. Default (nullable) if the task fails.</returns>
        public static async Task<TResult> TryOrSkip<TResult>(this Task<TResult> task)
        {
            try
            {
                return await task;
            }
            catch
            {
                // todo fix null silencing. Can't use a class constraint.
                return default!;
            }
        }
    }
}