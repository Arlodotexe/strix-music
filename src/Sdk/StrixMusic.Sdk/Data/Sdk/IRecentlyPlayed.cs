namespace StrixMusic.Sdk.Core.Data
{
    /// <inheritdoc cref="IPlaylistCollectionBase"/>
    /// <remarks>This interface should be implemented by the Sdk.</remarks>
    public interface IRecentlyPlayed : IRecentlyPlayedBase, IPlayableCollectionGroup, ISdkMember
    {
    }
}