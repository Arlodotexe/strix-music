using System;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace OwlCore.Extensions
{
    public static partial class AsyncExtensions
    {
        /// <summary>
        /// <inheritdoc cref="Threading.Debounce"/>
        /// </summary>
        /// <param name="task">The task to execute.</param>
        /// <param name="id">A unique identifier for this task / context.</param>
        /// <param name="timeToWait">The time to wait after this has stopped being called before running the task.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        public static async Task Debounce(this Task task, string id, TimeSpan timeToWait)
        {
            if (await Threading.Debounce(id, timeToWait))
                await task;
        }
    }
}
