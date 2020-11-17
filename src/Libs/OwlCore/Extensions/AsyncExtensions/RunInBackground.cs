using System;
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
    }
}
