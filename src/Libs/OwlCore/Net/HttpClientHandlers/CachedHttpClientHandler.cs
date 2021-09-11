using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace OwlCore.Net.HttpClientHandlers
{
    /// <summary>
    /// An <see cref="HttpClientHandler"/> that caches requests and returns them when specified (with optional cache filtering).
    /// </summary>
    public class CachedHttpClientHandler : HttpClientHandler
    {
        private readonly CachedHttpClientHandlerAction _cachedHttpClientHandler;

        /// <summary>
        /// Creates an instance of the <see cref="CachedHttpClientHandlerAction"/>.
        /// </summary>
        public CachedHttpClientHandler(string cacheFolderPath, TimeSpan defaultCacheTime)
        {
            _cachedHttpClientHandler = new CachedHttpClientHandlerAction(cacheFolderPath, defaultCacheTime);
        }

        /// <inheritdoc />
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return _cachedHttpClientHandler.SendAsync(request, cancellationToken, () => base.SendAsync(request, cancellationToken));
        }
    }
}