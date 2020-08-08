using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OwlCore.Extensions
{
    public static partial class AsyncExtensions
    {
        /// <summary>
        /// Runs a specific task in parallel from a list of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to operate on.</typeparam>
        /// <param name="items">The source items.</param>
        /// <param name="func">Returns the task to run in parallel from <typeparamref name="T"/>.</param>
        /// <returns>A task representing the completion of all tasks.</returns>
        public static Task InParallel<T>(this IEnumerable<T> items, Func<T, Task> func)
        {
            var tasks = items.Select(func);
            if (tasks == null)
                return Task.CompletedTask;

            return Task.WhenAll(tasks);
        }
    }
}
