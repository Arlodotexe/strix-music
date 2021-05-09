using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace OwlCore.Net.HttpClientHandlers
{
    /// <summary>
    /// An <see cref="HttpClientHandler"/> that provides rate limiting functionality.
    /// </summary>
    public class CachedHttpClientHandler : HttpClientHandler
    {
        private readonly CachedHttpClientHandlerAction _rateLimiterAction;

        /// <summary>
        /// Creates an instance of the <see cref="CachedHttpClientHandlerAction"/>.
        /// </summary>
        public CachedHttpClientHandler(string cacheFolderPath, TimeSpan defaultCacheTime)
        {
            _rateLimiterAction = new CachedHttpClientHandlerAction(cacheFolderPath, defaultCacheTime);
        }

        /// <summary>
        /// Creates an instance of the <see cref="CachedHttpClientHandlerAction"/>.
        /// </summary>
        public CachedHttpClientHandler(string cacheFolderPath, TimeSpan defaultCacheTime, CacheRequestFilter cacheRequestFilter)
        {
            _rateLimiterAction = new CachedHttpClientHandlerAction(cacheFolderPath, defaultCacheTime, cacheRequestFilter);
        }

        /// <inheritdoc />
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return _rateLimiterAction.SendAsync(request, cancellationToken, () => base.SendAsync(request, cancellationToken));
        }
    }
}