using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace OwlCore.Net.HttpClientHandlers
{
    /// <summary>
    /// An <see cref="HttpClientHandler"/> that provides rate limiting functionality.
    /// </summary>
    public class RateLimitedHttpClientHandler : HttpClientHandler
    {
        private readonly RateLimitedHttpClientHandlerAction _rateLimiterAction;

        /// <summary>
        /// Creates an instance of the <see cref="RateLimitedHttpClientHandler"/>.
        /// </summary>
        /// <param name="cooldownWindowTimeSpan">The amount of time before the cooldown window resets. Requests that are this old no longer count towards <paramref name="maxNumberOfRequestsPerCooldownWindow"/>.</param>
        /// <param name="maxNumberOfRequestsPerCooldownWindow">The maximum number of requests allowed per cooldown window.</param>
        public RateLimitedHttpClientHandler(TimeSpan cooldownWindowTimeSpan, int maxNumberOfRequestsPerCooldownWindow)
        {
            _rateLimiterAction = new RateLimitedHttpClientHandlerAction(cooldownWindowTimeSpan, maxNumberOfRequestsPerCooldownWindow);
        }

        /// <inheritdoc />
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return _rateLimiterAction.SendAsync(request, cancellationToken, () => base.SendAsync(request, cancellationToken));
        }
    }
}
