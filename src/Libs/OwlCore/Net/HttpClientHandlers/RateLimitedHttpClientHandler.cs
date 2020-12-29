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
        public RateLimitedHttpClientHandler(TimeSpan limitCooldown, int maxNumberOfRequests)
        {
            _rateLimiterAction = new RateLimitedHttpClientHandlerAction(limitCooldown, maxNumberOfRequests);
        }

        /// <inheritdoc />
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return _rateLimiterAction.SendAsync(request, cancellationToken, () => base.SendAsync(request, cancellationToken));
        }
    }
}
