using StrixMusic.Sdk.Models.Base;

namespace StrixMusic.Sdk.Models.Core
{
    /// <summary>
    /// Contains recently played albums, artists, tracks, playlists, etc.
    /// </summary>
    /// <remarks>This interface should be implemented by a core.</remarks>
    public interface ICoreRecentlyPlayed : IRecentlyPlayedBase, ICorePlayableCollectionGroup, ICoreMember
    {
    }
}
