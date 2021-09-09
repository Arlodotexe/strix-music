using System;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace OwlCore
{
    /// <summary>
    /// Helper methods related to code flow.
    /// </summary>
    public static partial class Flow
    {
        /// <summary>
        /// Syntactic sugar for catching any exception that may occur in a <see cref="Task"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        public static void TryOrSkip<TException>(Action action)
            where TException : Exception
        {
            try
            {
                action();
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
        public static TResult TryOrSkip<TException, TResult>(Func<TResult> task)
            where TException : Exception
            where TResult : class
        {
            try
            {
                return task();
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
        public static TResult TryOrSkip<TResult>(Func<TResult> task)
        {
            try
            {
                return task();
            }
            catch
            {
                // todo fix null silencing. Can't use a class constraint.
                return default!;
            }
        }
    }
}