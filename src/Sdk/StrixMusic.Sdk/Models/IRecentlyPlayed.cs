using StrixMusic.Sdk.Models.Base;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Models.Merged;

namespace StrixMusic.Sdk.Models
{
    /// <summary>
    /// Contains recently played albums, artists, tracks, playlists, etc.
    /// </summary>
    /// <remarks>Instances of this class may contain data merged from one or more sources.</remarks>
    public interface IRecentlyPlayed : IRecentlyPlayedBase, IPlayableCollectionGroup, IPlayable, ISdkMember, IMerged<ICoreRecentlyPlayed>
    {
    }
}