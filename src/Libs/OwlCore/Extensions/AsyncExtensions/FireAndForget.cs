using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace OwlCore.Extensions
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