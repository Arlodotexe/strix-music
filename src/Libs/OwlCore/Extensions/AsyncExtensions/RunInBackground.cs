using System;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace OwlCore.Extensions
{
    public static partial class AsyncExtensions
    {
        /// <summary>
        /// Wraps a given task in <see cref="Task.Run(Func{Task})"/>
        /// </summary>
        /// <param name="task">The task to execute.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        public static Task RunInBackground(this Task task)
        {
            return Task.Run(async () => await task);
        }

        /// <summary>
        /// Wraps a given task in <see cref="Task.Run(Func{Task})"/>
        /// </summary>
        /// <param name="task">The task to execute.</param>
        /// <param name="cancellationToken">A cancellation token for this the returned task.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        public static Task RunInBackground(this Task task, CancellationToken cancellationToken)
        {
            return Task.Run(async () => await task, cancellationToken);
        }

        /// <summary>
        /// Wraps a given task in <see cref="Task.Run(Func{Task})"/>
        /// </summary>
        /// <param name="task">The task to execute.</param>
        /// <param name="cancellationToken">A cancellation token for this the returned task.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        public static Task<T> RunInBackground<T>(this Task<T> task, CancellationToken cancellationToken)
        {
            return Task.Run(async () => await task, cancellationToken);
        }
    }
}
