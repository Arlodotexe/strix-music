using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.Extensions;

namespace OwlCore.Net.HttpClientHandlers
{
    /// <summary>
    /// An <see cref="CompositeHttpClientHandlerActionBase"/> that provides caching functionality.
    /// </summary>
    public class CachedHttpClientHandlerAction : CompositeHttpClientHandlerActionBase
    {
        private readonly string _cacheFolderPath;
        private readonly TimeSpan _defaultCacheTime;

        /// <summary>
        /// Creates an instance of the <see cref="CachedHttpClientHandlerAction"/>.
        /// </summary>
        public CachedHttpClientHandlerAction(string cacheFolderPath, TimeSpan defaultCacheTime)
        {
            _cacheFolderPath = cacheFolderPath;
            _defaultCacheTime = defaultCacheTime;
        }

        /// <summary>
        /// Raised when cache is found and is about to be used.
        /// </summary>
        public event EventHandler<CachedRequestEventArgs>? CachedRequestFound;

        /// <summary>
        /// Raised when new data is retrieved and is about to be saved.
        /// </summary>
        public event EventHandler<CachedRequestEventArgs>? CachedRequestSaving;

        /// <inheritdoc cref="HttpClientHandler.SendAsync(HttpRequestMessage, CancellationToken)"/>
        public override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken, Func<Task<HttpResponseMessage>> baseSendAsync)
        {
            var path = Path.GetFullPath(_cacheFolderPath);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            // check if item is cached
            var cachedData = ReadCachedFile(path, request.RequestUri.AbsoluteUri);

            var shouldUseCache = true;
            if (cachedData != null)
            {
                var requestFoundEventArgs = new CachedRequestEventArgs(request.RequestUri, cachedData);
                CachedRequestFound?.Invoke(this, requestFoundEventArgs);

                shouldUseCache = !requestFoundEventArgs.Handled;
            }

            if (cachedData != null && shouldUseCache)
            {
                // if cache found and not expired
                if (cachedData.TimeStamp + _defaultCacheTime > DateTime.UtcNow && cachedData.ContentBytes != null)
                {
                    var response = new HttpResponseMessage(HttpStatusCode.OK);
                    response.Content = new ByteArrayContent(cachedData.ContentBytes);

                    return response;
                }

                // If expired, remove entry.
                var cachedFilePath = GetCachedFilePath(path, request.RequestUri.AbsoluteUri);

                try
                {
                    File.Delete(cachedFilePath);
                }
                catch
                {
                    // ignored
                }
            }

            var result = await baseSendAsync();
            var freshCacheData = await CreateCachedData(request.RequestUri.AbsoluteUri, result);

            var shouldSaveEventArgs = new CachedRequestEventArgs(request.RequestUri, freshCacheData);
            CachedRequestSaving?.Invoke(this, shouldSaveEventArgs);

            if (!shouldSaveEventArgs.Handled)
                WriteCachedFile(path, freshCacheData);

            return result;
        }

        /// <summary>
        /// Creates cached data.
        /// </summary>
        /// <param name="request">API request information.</param>
        /// <param name="response">The response string to be cached.</param>
        /// <returns>Returns a <see cref="Task" /> that represents the asynchronous operation.</returns>
        public static async Task<CacheEntry> CreateCachedData(string request, HttpResponseMessage response)
        {
            var contentBytes = await response.Content.ReadAsByteArrayAsync();

            return new CacheEntry
            {
                ContentBytes = contentBytes,
                RequestUri = request,
                TimeStamp = DateTime.UtcNow,
            };
        }

        /// <summary>
        /// Writes cache to the file.
        /// </summary>
        /// <param name="path">Path to cache file.</param>
        /// <param name="cacheEntry">The cache data to write to disk.</param>
        /// <returns>Returns a <see cref="Task" /> that represents the asynchronous operation.</returns>
        public static void WriteCachedFile(string path, CacheEntry cacheEntry)
        {
            Guard.IsNotNull(cacheEntry.RequestUri, nameof(cacheEntry.RequestUri));

            var cachedFilePath = GetCachedFilePath(path, cacheEntry.RequestUri);

            var serializedData = JsonSerializer.Serialize(cacheEntry);

            File.WriteAllText(cachedFilePath, serializedData);
        }

        /// <summary>
        /// Read cache data.
        /// </summary>
        /// <param name="path">Path to the cache file</param>
        /// <param name="request">API request information</param>
        /// <returns>Information related to cache in a <see cref="CacheEntry"/></returns>
        private static CacheEntry? ReadCachedFile(string path, string request)
        {
            var cachedFilePath = GetCachedFilePath(path, request);

            if (!File.Exists(cachedFilePath))
                return null;

            CacheEntry? cacheEntry = null;
            bool fileExists = false;
            try
            {
                fileExists = File.Exists(cachedFilePath);

                var fileBytes = File.ReadAllText(cachedFilePath);
                cacheEntry = JsonSerializer.Deserialize<CacheEntry>(fileBytes);
            }
            catch (Exception ex)
            {
                if (fileExists)
                    Debug.WriteLine($"WARNING: Failed to read or deserialized the file at \"{cachedFilePath}\". The data will be discarded. ({ex})");
            }

            if (cacheEntry?.RequestUri is null)
                return null;

            // Check if the cached request matches the given (could be a hash collision).
            if (!request.Contains(cacheEntry.RequestUri))
                return null;

            return cacheEntry;
        }

        /// <summary>
        /// Generates a file for the cache.
        /// </summary>
        /// <param name="basePath">Path to the directory where the file is stored.</param>
        /// <param name="requestUri">The request uri.</param>
        /// <returns>The file path.</returns>
        private static string GetCachedFilePath(string basePath, string requestUri)
        {
            return Path.Combine(basePath, requestUri.HashMD5Fast()) + ".cache";
        }
    }

    /// <summary>
    /// A class to hold and save cached data.
    /// </summary>
    public class CacheEntry
    {
        /// <summary>
        /// The cached response object.
        /// </summary>
        public string? RequestUri { get; set; }

        /// <summary>
        /// The http response content.
        /// </summary>
        public byte[]? ContentBytes { get; set; }

        /// <summary>
        /// Timestamp for the cache.
        /// </summary>
        public DateTime TimeStamp { get; set; }
    }

    /// <summary>
    /// <see cref="EventArgs"/> used to handled if a request should be saved to disk or used in <see cref="CachedHttpClientHandlerAction"/>.
    /// </summary>
    public class CachedRequestEventArgs : EventArgs
    {
        /// <summary>
        /// Creates a new instance of <see cref="CachedRequestEventArgs"/>.
        /// </summary>
        public CachedRequestEventArgs(Uri requestUri, CacheEntry cacheEntry)
        {
            RequestUri = requestUri;
            CacheEntry = cacheEntry;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the event was handled.
        /// </summary>
        public bool Handled { get; set; }

        /// <summary>
        /// The uri of the current request.
        /// </summary>
        public Uri RequestUri { get; set; }

        /// <summary>
        /// The cached data, if present.
        /// </summary>
        public CacheEntry CacheEntry { get; set; }
    }
}