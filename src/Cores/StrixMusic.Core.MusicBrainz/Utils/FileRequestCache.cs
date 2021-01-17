using System;
using System.IO;
using System.Threading.Tasks;
using Hqub.MusicBrainz.API.Cache;
using OwlCore;

namespace StrixMusic.Core.MusicBrainz.Utils
{
    /// <summary>
    /// Caches requests to MusicBrainz API on disk.
    /// </summary>
    public class FileRequestCache : IRequestCache
    {
        private readonly string _path;

        /// <summary>
        /// Gets or sets the timeout for a cache entry to expire.
        /// </summary>
        public TimeSpan ExpiresIn { get; set; } = TimeSpan.FromHours(48.0);

        /// <summary>
        /// Initializes a new instance of the <see cref="FileRequestCache"/> class.
        /// </summary>
        /// <param name="path"></param>
        public FileRequestCache(string path)
        {
            _path = Path.GetFullPath(path);

            Flow.TryOrSkip<Exception>(() => _ = Task.Run(Cleanup));
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
            stream = null;

            var item = CacheEntry.Read(_path, request);
            if (item == null)
            {
                return Task.FromResult(false);
            }

            if (DateTime.Now - item.TimeStamp > ExpiresIn)
            {
                item.Stream?.Close();
                return Task.FromResult(false);
            }

            stream = item.Stream;

            return Task.FromResult(true);
        }

        /// <summary>
        /// Cleans out cached files that are passed their <see cref="ExpiresIn"/>.
        /// </summary>
        public void Cleanup()
        {
            if (!Directory.Exists(_path))
                return;

            var files = Directory.EnumerateFiles(_path, "*.mb-cache");

            foreach (var file in files)
            {
                if (DateTime.Now - CacheEntry.GetTimestamp(file) > ExpiresIn)
                {
                    File.Delete(file);
                }
            }
        }

        /// <summary>
        /// Clears out all cached files.
        /// </summary>
        public void Clear()
        {
            if (!Directory.Exists(_path))
                return;

            foreach (var file in Directory.EnumerateFiles(_path, "*.mb-cache"))
            {
                File.Delete(file);
            }
        }
    }
}
