namespace StrixMusic.Sdk.Services.StorageService
{
    /// <inheritdoc />
    public class DefaultCacheService : CacheServiceBase
    {
        /// <inheritdoc />
        public DefaultCacheService(IFileSystemService fileSystemService)
            : base(fileSystemService)
        {
        }

        /// <inheritdoc />
        public override string Id { get; protected set; } = "Default";
    }
}