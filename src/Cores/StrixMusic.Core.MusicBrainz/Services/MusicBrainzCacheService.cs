using StrixMusic.Sdk.Services.StorageService;

namespace StrixMusic.Core.MusicBrainz.Services
{
    /// <summary>
    /// A caching service for web API
    /// </summary>
    public class MusicBrainzCacheService : CacheServiceBase
    {
        /// <inheritdoc />
        public override string Id { get; protected set; } = nameof(MusicBrainzCacheService);
    }
}
