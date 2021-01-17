using System;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace OwlCore.Extensions
{
    public static partial class AsyncExtensions
    {
        /// <summary>
        /// Invokes a task on the UI thread.
        /// </summary>
        /// <param name="task">The task to invoke.</param>
        public static async Task InvokeOnUI(this Task task)
        {
            var originalContext = SynchronizationContext.Current;

            if (Threading.PrimarySyncContext is null)
                throw new InvalidOperationException($"UI context not found. {nameof(Threading.SetPrimarySynchronizationContext)} was not called.");

            SynchronizationContext.SetSynchronizationContext(Threading.PrimarySyncContext);

            await task;

            SynchronizationContext.SetSynchronizationContext(originalContext);
        }
    }
}