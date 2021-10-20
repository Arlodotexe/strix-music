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

            _cachedHttpClientHandler.CachedRequestFound += OnCachedRequestFound;
            _cachedHttpClientHandler.CachedRequestSaving += OnCachedRequestSaving;
        }

        private void OnCachedRequestSaving(object sender, CachedRequestEventArgs e)
        {
            CachedRequestSaving?.Invoke(sender, e);
        }

        private void OnCachedRequestFound(object sender, CachedRequestEventArgs e)
        {
            CachedRequestFound?.Invoke(sender, e);
        }

        /// <summary>
        /// Raised when cache is found and is about to be used.
        /// </summary>
        public event EventHandler<CachedRequestEventArgs>? CachedRequestFound;

        /// <summary>
        /// Raised when new data is retrieved and is about to be saved.
        /// </summary>
        public event EventHandler<CachedRequestEventArgs>? CachedRequestSaving;

        /// <inheritdoc />
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return _cachedHttpClientHandler.SendAsync(request, cancellationToken, () => base.SendAsync(request, cancellationToken));
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            _cachedHttpClientHandler.CachedRequestFound -= OnCachedRequestFound;
            _cachedHttpClientHandler.CachedRequestSaving -= OnCachedRequestSaving;

            base.Dispose(disposing);
        }
    }
}