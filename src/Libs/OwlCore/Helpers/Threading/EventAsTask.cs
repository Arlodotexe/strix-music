using System;
using System.Threading;
using System.Threading.Tasks;
using OwlCore.Extensions;

// ReSharper disable once CheckNamespace
namespace OwlCore.Helpers
{
    /// <summary>
    /// Helpers related to Threading.
    /// </summary>
    public static partial class Threading
    {
        /// <summary>
        /// Waits for an event to fire.
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        public static async Task<(object? Sender, TResult Result)> EventAsTask<TResult>(Action<EventHandler<TResult>> subscribe, Action<EventHandler<TResult>> unsubscribe, TimeSpan timeout)
        {
            var resultCancellationToken = new CancellationTokenSource();
            resultCancellationToken.CancelAfter(timeout);

            var completionSource = new TaskCompletionSource<(object? Sender, TResult Result)>();

            void EventHandler(object sender, TResult eventArgs)
            {
                completionSource?.SetResult((sender, eventArgs));
            }

            subscribe(EventHandler);

            var result = await completionSource.Task
                .RunInBackground(resultCancellationToken.Token)
                .TryOrSkip();

            unsubscribe(EventHandler);

            return result;
        }
    }
}