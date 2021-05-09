using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace OwlCore
{
    public static partial class Flow
    {
        private static readonly ConcurrentDictionary<string, DebouncerData> _inUseDebouncers = new ConcurrentDictionary<string, DebouncerData>();

        /// <summary>
        /// Debouncing enforces that a function not be called again until a certain amount of time has passed without it being called.
        /// As in “execute this function only if 100 milliseconds have passed without it being called.”. 
        /// </summary>
        /// <remarks> This is especially useful when you have an event that fires repeatedly. but you only care about when the event stops being called.
        /// </remarks>
        /// <returns>False if debouce was triggered, True if no debounce occured and "post debounce" code should execute.</returns>
        public static async Task<bool> Debounce(string debouncerKey, TimeSpan timeToWait)
        {
            // If the debouncer data doesn't exist
            if (!_inUseDebouncers.TryGetValue(debouncerKey, out var debouncerData))
            {
                // Create new debounce data.
                _inUseDebouncers.TryAdd(debouncerKey, debouncerData ??= new DebouncerData());
            }
            else
            {
                // If the debouncer exists already
                // cancel the previous Delay and create a new token for the next one.
                debouncerData.TokenSource.Cancel();
                debouncerData.TokenSource = new CancellationTokenSource();
            }

            await debouncerData.Lock.WaitAsync();

            // If the token was already canceled (and not reset) by the time we get here, don't await.
            if (debouncerData.TokenSource.IsCancellationRequested)
            {
                Cleanup();
                return false;
            }

            try
            {
                await Task.Delay(timeToWait, debouncerData.TokenSource.Token);
                Cleanup();

                return true;
            }
            catch (TaskCanceledException)
            {
                debouncerData.Lock.Release();
                return false;
            }

            void Cleanup()
            {
                debouncerData.Lock.Release();
                _inUseDebouncers.TryRemove(debouncerKey, out _);
            }
        }

        // TODO: Make a record, when C# 9 is enabled
        private class DebouncerData
        {
            /// <summary>
            /// The Async Lock used to keep debounce queries from interfering with each other.
            /// </summary>
            public SemaphoreSlim Lock { get; } = new SemaphoreSlim(1, 1);

            /// <summary>
            /// The <see cref="CancellationTokenSource"/> used to cancel a debounce task when reset.
            /// </summary>
            public CancellationTokenSource TokenSource { get; set; } = new CancellationTokenSource();
        }
    }
}
