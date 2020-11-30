using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace OwlCore.Net.HttpClientHandlers
{
    /// <summary>
    /// An <see cref="HttpClientHandler"/> that provides rate limiting functionality.
    /// </summary>
    public class RateLimitedHttpClientHandler : HttpClientHandler
    {
        private readonly TimeSpan _limitCooldown;
        private readonly int _maxNumberOfRequests;
        private static readonly AsyncLock _mutex = new AsyncLock();
        private static readonly Queue<DateTime> _requestTimestampsInCooldownWindow = new Queue<DateTime>();

        /// <summary>
        /// Creates an instance of the <see cref="RateLimitedHttpClientHandler"/>.
        /// </summary>
        public RateLimitedHttpClientHandler(TimeSpan limitCooldown, int maxNumberOfRequests)
        {
            _limitCooldown = limitCooldown;
            _maxNumberOfRequests = maxNumberOfRequests;
        }


        /// <inheritdoc />
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            using (await _mutex.LockAsync())
            {
                // store the timestamp of the request being made
                _requestTimestampsInCooldownWindow.Enqueue(DateTime.Now);

                // if a request is older than now - cooldown, it's outside the cooldown window and should be removed.
                while (_requestTimestampsInCooldownWindow.Count > 0 &&
                DateTime.Now.Subtract(_limitCooldown).Ticks > _requestTimestampsInCooldownWindow.Peek().Ticks)
                {
                    _requestTimestampsInCooldownWindow.Dequeue();
                }

                // if there are too many requests in the current cooldown window
                // then delay [time window] - [amount of time passed since the request was made] - remaining time until the oldest request goes outside the cooldown window
                if (_requestTimestampsInCooldownWindow.Count > _maxNumberOfRequests)
                {
                    var timeSinceOldestRequestWasMade = DateTime.Now - _requestTimestampsInCooldownWindow.Peek();

                    await Task.Delay(_limitCooldown - timeSinceOldestRequestWasMade);
                }

                return await base.SendAsync(request, cancellationToken);
            }
        }
    }
}
