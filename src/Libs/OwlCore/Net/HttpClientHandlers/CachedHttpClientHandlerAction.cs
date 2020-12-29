using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace OwlCore.Net.HttpClientHandlers
{
    /// <summary>
    /// An <see cref="CompositeHttpClientHandlerActionBase"/> that provides caching functionality.
    /// </summary>
    public class CachedHttpClientHandlerAction : CompositeHttpClientHandlerActionBase
    {
        private readonly string _cacheFolderPath;
        private readonly TimeSpan _cacheTime;
        private readonly CacheRequestFilter? _cacheRequestFilter;

        /// <summary>
        /// Decides if the given URL should return data from cache.
        /// </summary>
        /// <param name="uri">The URL to decide against.</param>
        /// <returns><see langword="true"/> if values should be cached and returned, otherwise false.</returns>
        public delegate bool CacheRequestFilter(Uri uri);

        /// <summary>
        /// Creates an instance of the <see cref="RateLimitedHttpClientHandlerAction"/>.
        /// </summary>
        public CachedHttpClientHandlerAction(string cacheFolderPath, TimeSpan cacheTime)
        {
            _cacheFolderPath = cacheFolderPath;
            _cacheTime = cacheTime;
        }

        /// <summary>
        /// Creates an instance of the <see cref="RateLimitedHttpClientHandlerAction"/>.
        /// </summary>
        public CachedHttpClientHandlerAction(string cacheFolderPath, TimeSpan cacheTime, CacheRequestFilter cacheRequestFilter)
        {
            _cacheFolderPath = cacheFolderPath;
            _cacheTime = cacheTime;
            _cacheRequestFilter = cacheRequestFilter;
        }

        /// <inheritdoc cref="HttpClientHandler.SendAsync(HttpRequestMessage, CancellationToken)"/>
        public override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken, Func<Task<HttpResponseMessage>> baseSendAsync)
        {
            if (_cacheRequestFilter?.Invoke(request.RequestUri) ?? false)
            {
                var path = Path.GetFullPath(_cacheFolderPath);

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                // check if item is cached
                var cachedEntry = ReadCachedFile(path, request.RequestUri.AbsoluteUri);
                if (cachedEntry != null)
                {
                    // if cache found and not expired
                    if (cachedEntry.TimeStamp + _cacheTime > DateTime.UtcNow && cachedEntry.ContentBytes != null)
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

                // if not found, get data and cache it
                var result = await baseSendAsync();

                await WriteCachedFile(path, request.RequestUri.AbsoluteUri, result);

                return result;
            }

            return await baseSendAsync();
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

            try
            {
                var fileBytes = File.ReadAllText(cachedFilePath);
                cacheEntry = JsonSerializer.Deserialize<CacheEntry>(fileBytes);
            }
            catch
            {
                // ignored
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
            return Path.Combine(basePath, GetHash(requestUri)) + ".cache";
        }

        /// <summary>
        /// Returns hash of a string (based on MD5, but only 16 instead of 32 bytes).
        /// </summary>
        /// <param name="seed">Input string.</param>
        /// <returns>MD5 hash.</returns>
        private static string GetHash(string seed)
        {
            unchecked
            {
                int hash = 23;

                foreach (char c in seed)
                {
                    hash = hash * 31 + c;
                }

                return Convert.ToString(hash, 16);
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
    }
}