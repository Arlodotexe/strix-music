namespace StrixMusic.Sdk.Services.StorageService
{
    /// <inheritdoc />
    public sealed class DefaultCacheService : CacheServiceBase
    {
        /// <inheritdoc />
        public override string Id { get; protected set; } = "Default";
    }
}