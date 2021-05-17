using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
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
        private readonly CacheRequestFilter? _shouldReturnFromCacheFilter;
        private readonly CacheRequestFilter? _shouldSaveToCacheFilter;

        /// <summary>
        /// Creates an instance of the <see cref="CachedHttpClientHandlerAction"/>.
        /// </summary>
        public CachedHttpClientHandlerAction(string cacheFolderPath, TimeSpan defaultCacheTime)
        {
            _cacheFolderPath = cacheFolderPath;
            _defaultCacheTime = defaultCacheTime;
        }

        /// <summary>
        /// Creates an instance of the <see cref="CachedHttpClientHandlerAction"/>.
        /// </summary>
        public CachedHttpClientHandlerAction(string cacheFolderPath, TimeSpan defaultCacheTime, CacheRequestFilter shouldReturnFromCacheFilter)
        {
            _cacheFolderPath = cacheFolderPath;
            _defaultCacheTime = defaultCacheTime;
            _shouldReturnFromCacheFilter = shouldReturnFromCacheFilter;
        }

        /// <summary>
        /// Creates an instance of the <see cref="CachedHttpClientHandlerAction"/>.
        /// </summary>
        public CachedHttpClientHandlerAction(string cacheFolderPath, TimeSpan defaultCacheTime, CacheRequestFilter shouldReturnFromCacheFilter, CacheRequestFilter shouldSaveToCacheFilter)
        {
            _cacheFolderPath = cacheFolderPath;
            _defaultCacheTime = defaultCacheTime;
            _shouldReturnFromCacheFilter = shouldReturnFromCacheFilter;
            _shouldSaveToCacheFilter = shouldSaveToCacheFilter;
        }

        /// <inheritdoc cref="HttpClientHandler.SendAsync(HttpRequestMessage, CancellationToken)"/>
        public override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken, Func<Task<HttpResponseMessage>> baseSendAsync)
        {
            var path = Path.GetFullPath(_cacheFolderPath);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            // check if item is cached
            var cachedEntry = ReadCachedFile(path, request.RequestUri.AbsoluteUri);

            var shouldUseCache = _shouldReturnFromCacheFilter?.Invoke(request.RequestUri, cachedEntry) ?? false;

            if (cachedEntry != null && shouldUseCache)
            {
                // if cache found and not expired
                if (cachedEntry.TimeStamp + _defaultCacheTime > DateTime.UtcNow && cachedEntry.ContentBytes != null)
                {
                    var response = new HttpResponseMessage(HttpStatusCode.OK);
                    response.Content = new ByteArrayContent(cachedEntry.ContentBytes);

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

            if (_shouldSaveToCacheFilter == null || _shouldSaveToCacheFilter.Invoke(request.RequestUri, cachedEntry))
                await WriteCachedFile(path, request.RequestUri.AbsoluteUri, result);

            return result;
        }

        /// <summary>
        /// Writes cache to the file.
        /// </summary>
        /// <param name="path">Path to cache file.</param>
        /// <param name="request">API request information.</param>
        /// <param name="response">The response string to be cached.</param>
        /// <returns>Returns a <see cref="Task" /></returns>
        public static async Task WriteCachedFile(string path, string request, HttpResponseMessage response)
        {
            var cachedFilePath = GetCachedFilePath(path, request);
            var contentBytes = await response.Content.ReadAsByteArrayAsync();

            var cacheEntry = new CacheEntry
            {
                ContentBytes = contentBytes,
                RequestUri = request,
                TimeStamp = DateTime.UtcNow,
            };

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
    /// Decides if the given URL should return data from cache.
    /// </summary>
    /// <param name="uri">The URL to decide against.</param>
    /// <param name="cacheEntry">The cache entry for this request, if found.</param>
    /// <returns><see langword="true"/> if a cached value should be returned/cached, otherwise false.</returns>
    public delegate bool CacheRequestFilter(Uri uri, CacheEntry? cacheEntry = null);

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
}