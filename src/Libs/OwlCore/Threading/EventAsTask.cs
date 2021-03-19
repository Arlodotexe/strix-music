using System;
using System.Threading;
using System.Threading.Tasks;
using OwlCore.Extensions;

// ReSharper disable once CheckNamespace
namespace OwlCore
{
    /// <summary>
    /// Helpers related to Threading.
    /// </summary>
    public static partial class Threading
    {
        /// <summary>
        /// Waits for an event to fire. If the event doesn't fire within the given timeout, a default value is returned.
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        public static async Task<(object? Sender, TResult Result)?> EventAsTask<TResult>(Action<EventHandler<TResult>> subscribe, Action<EventHandler<TResult>> unsubscribe, TimeSpan timeout)
        {
            var completionSource = new TaskCompletionSource<(object? Sender, TResult Result)>();
            var resultCancellationToken = new CancellationTokenSource();

            resultCancellationToken.CancelAfter(timeout);
            subscribe(EventHandler);

            try
            {
                var result = await Task.Run(() => completionSource.Task, resultCancellationToken.Token);
                unsubscribe(EventHandler);
                return result;
            }
            catch (TaskCanceledException)
            {
                unsubscribe(EventHandler);
                return null;
            }

            void EventHandler(object sender, TResult eventArgs)
            {
                completionSource.SetResult((sender, eventArgs));
            }
        }

        /// <summary>
        /// Waits for an event to fire. If the event doesn't fire within the given timeout, a default value is returned.
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        public static async Task<(object? Sender, EventArgs Result)?> EventAsTask(Action<EventHandler> subscribe, Action<EventHandler> unsubscribe, TimeSpan timeout)
        {
            var completionSource = new TaskCompletionSource<(object? Sender, EventArgs Result)>();
            var resultCancellationToken = new CancellationTokenSource();

            resultCancellationToken.CancelAfter(timeout);
            subscribe(EventHandler);

            try
            {
                var result = await Task.Run(() => completionSource.Task, resultCancellationToken.Token);
                unsubscribe(EventHandler);
                return result;
            }
            catch (TaskCanceledException)
            {
                unsubscribe(EventHandler);
                return null;
            }

            void EventHandler(object sender, EventArgs eventArgs)
            {
                completionSource.SetResult((sender, eventArgs));
            }
        }
    }
}