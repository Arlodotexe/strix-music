using StrixMusic.Sdk.Data.Base;

namespace StrixMusic.Sdk.Data.Core
{
    /// <inheritdoc cref="IPlaylistCollectionBase"/>
    /// <remarks>This interface should be implemented by a core.</remarks>
    public interface ICoreRecentlyPlayed : IRecentlyPlayedBase, ICorePlayableCollectionGroup, ICoreMember
    {
    }
}
