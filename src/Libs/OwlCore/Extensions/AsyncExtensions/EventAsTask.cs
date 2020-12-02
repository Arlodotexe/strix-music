using System;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace OwlCore.Extensions
{
    public static partial class AsyncExtensions
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