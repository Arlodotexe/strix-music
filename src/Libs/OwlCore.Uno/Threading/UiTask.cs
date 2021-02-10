using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace OwlCore.Uno.Threading
{
    /// <summary>
    /// When used as a return type, the continuation happens on the UI thread.
    /// </summary>
    [AsyncMethodBuilder(typeof(UiTaskMethodBuilder))]
    public class UiTask
    {
        internal TaskCompletionSource<object> Promise { get; } = new TaskCompletionSource<object>();

        /// <summary>
        /// Returns a <see cref="Task"/> that represents the asynchronous operation.
        /// </summary>
        public Task AsTask() => Promise.Task;

        /// <inheritdoc cref="Task.GetAwaiter" />
        public TaskAwaiter<object> GetAwaiter()
        {
            return Promise.Task.GetAwaiter();
        }

        /// <summary>
        /// Implicit conversion to a <see cref="Task"/>.
        /// </summary>
        /// <param name="task">The task to convert.</param>
        public static implicit operator Task(UiTask task) => task.AsTask();
    }
}