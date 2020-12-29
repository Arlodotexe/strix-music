using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace OwlCore.Net.HttpClientHandlers
{
    /// <summary>
    /// A rate-limiting action holder for use in a <see cref="CompositeHttpClientHandler"/>.
    /// </summary>
    public class RateLimitedHttpClientHandlerAction : CompositeHttpClientHandlerActionBase
    {
        private readonly TimeSpan _cooldownWindowTimeSpan;
        private readonly int _maxNumberOfRequestsPerCooldownWindow;
        private static readonly AsyncLock _mutex = new AsyncLock();
        private static readonly Queue<DateTime> _requestTimestampsInCooldownWindow = new Queue<DateTime>();

        /// <summary>
        /// Creates a new instance of <see cref="RateLimitedHttpClientHandlerAction"/>.
        /// </summary>
        /// <param name="cooldownWindowTimeSpan">The amount of time before the cooldown window resets. Requests that are this old no longer count towards <paramref name="maxNumberOfRequestsPerCooldownWindow"/>.</param>
        /// <param name="maxNumberOfRequestsPerCooldownWindow">The maximum number of requests allowed per cooldown window.</param>
        public RateLimitedHttpClientHandlerAction(TimeSpan cooldownWindowTimeSpan, int maxNumberOfRequestsPerCooldownWindow)
        {
            _cooldownWindowTimeSpan = cooldownWindowTimeSpan;
            _maxNumberOfRequestsPerCooldownWindow = maxNumberOfRequestsPerCooldownWindow;
        }

        /// <inheritdoc />
        public override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken, Func<Task<HttpResponseMessage>> baseSendAsync)
        {
            using (await _mutex.LockAsync())
            {
                // store the timestamp of the request being made
                _requestTimestampsInCooldownWindow.Enqueue(DateTime.Now);

                // if a request is older than now - cooldown, it's outside the cooldown window and should be removed.
                while (_requestTimestampsInCooldownWindow.Count > 0 && DateTime.Now.Subtract(_cooldownWindowTimeSpan).Ticks > _requestTimestampsInCooldownWindow.Peek().Ticks)
                {
                    _requestTimestampsInCooldownWindow.Dequeue();
                }

                // if there are too many requests in the current cooldown window
                // then delay [time window] - [amount of time passed since the request was made] - remaining time until the oldest request goes outside the cooldown window
                if (_requestTimestampsInCooldownWindow.Count > _maxNumberOfRequestsPerCooldownWindow)
                {
                    var timeSinceOldestRequestWasMade = DateTime.Now - _requestTimestampsInCooldownWindow.Peek();

                    await Task.Delay(_cooldownWindowTimeSpan - timeSinceOldestRequestWasMade, cancellationToken);
                }

                return await baseSendAsync();
            }
        }
    }
}