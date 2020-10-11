using System.Threading;
using System.Threading.Tasks;

namespace OwlCore.Extensions.AsyncExtensions
{
    public static partial class AsyncExtensions
    {
        /// <summary>
        /// Runs a <see cref="Task"/> in a fire-and-forget manner.
        /// </summary>
        /// <param name="task">The task to fire and forget.</param>
        public static void FireAndForget(this Task task)
        {
        }
    }
}