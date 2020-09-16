using System;
using System.IO;
using System.Threading.Tasks;
using Hqub.MusicBrainz.API.Cache;
using StrixMusic.Core.MusicBrainz.Statics;

namespace Hqub.MusicBrainz.Client
{
    /// <summary>
    /// Caches requests to MusicBrainz API on disk.
    /// </summary>
    public class FileRequestCache : IRequestCache
    {
        private const int HEADER_LENGTH = 512;
        private readonly string _path;

        /// <summary>
        /// Gets or sets the timeout for a cache entry to expire.
        /// </summary>
        public TimeSpan Timeout { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileRequestCache"/> class.
        /// </summary>
        public FileRequestCache()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            this._path = Path.Combine(appData, "MusicBrainz", "Cache");
            this.Timeout = TimeSpan.FromHours(24.0);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileRequestCache"/> class.
        /// </summary>
        /// <param name="path"></param>
        public FileRequestCache(string path)
        {
            this._path = Path.GetFullPath(path);

            this.Timeout = TimeSpan.FromHours(24.0);
        }

        /// <inheritdoc/>
        public async Task Add(string request, Stream response)
        {
            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
            }

            await CacheEntry.Write(_path, request, response);
        }

        /// <inheritdoc/>
        public Task<bool> TryGetCachedItem(string request, out Stream? stream)
        {
            var item = CacheEntry.Read(_path, request);
            stream = null;
            if (item == null)
            {
                return Task.FromResult(false);
            }

            if ((DateTime.Now - item.TimeStamp) > Timeout)
            {
                item.Stream?.Close();
                return Task.FromResult(false);
            }

            stream = item.Stream;

            return Task.FromResult(true);
        }

        /// <inheritdoc/>
        public int Cleanup()
        {
            int count = 0;
            var now = DateTime.Now;
            foreach (var file in Directory.EnumerateFiles(_path, "*.mb-cache"))
            {
                if ((now - CacheEntry.GetTimestamp(file)) > Timeout)
                {
                    File.Delete(file);
                }
            }

            return count;
        }

        /// <inheritdoc/>
        public void Clear()
        {
            // If the path is used for cache only, we could just as well delete the directory.
            if (Directory.Exists(_path))
            {
                Directory.Delete(_path);
            }

            foreach (var file in Directory.EnumerateFiles(_path, "*.mb-cache"))
            {
                File.Delete(file);
            }
        }
    }
}
