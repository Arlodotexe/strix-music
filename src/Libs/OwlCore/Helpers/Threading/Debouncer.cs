using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OwlCore.Helpers
{
    public static partial class Threading
    {
        private static readonly Dictionary<string, DebouncerData> _inUseDebouncers = new Dictionary<string, DebouncerData>();

        private class DebouncerData
        {
            /// <summary>
            /// The Async Lock used to keep debounce queries from interfering with each other.
            /// </summary>
            public SemaphoreSlim Lock { get; } = new SemaphoreSlim(1, 1);

            /// <summary>
            /// The <see cref="CancellationTokenSource"/> used to cancel a debounce task when reset.
            /// </summary>
            public CancellationTokenSource TokenSource { get; } = new CancellationTokenSource();
        }

        /// <summary>
        /// Debouncing enforces that a function not be called again until a certain amount of time has passed without it being called. As in “execute this function only if 100 milliseconds have passed without it being called.”. 
        /// </summary>
        /// <remarks> This is especially useful when you have an event that fires repeatedly. but you only care about when the event stops being called.
        /// </remarks>
        public static async Task<bool> Debounce(string debouncerKey, TimeSpan timeToWait)
        {
            if (_inUseDebouncers.TryGetValue(debouncerKey, out var debouncerData))
            {
                await debouncerData.Lock.WaitAsync();
                debouncerData.TokenSource.Cancel();
            }
            else
            {
                debouncerData = new DebouncerData();
                await debouncerData.Lock.WaitAsync();
                _inUseDebouncers.Add(debouncerKey, debouncerData);
            }

            try
            {
                await Task.Delay(timeToWait, debouncerData.TokenSource.Token);
                debouncerData.Lock.Release();
                _inUseDebouncers.Remove(debouncerKey);
                return true;
            }
            catch (TaskCanceledException)
            {
                debouncerData.Lock.Release();
                return false;
            }
        }
    }
}
