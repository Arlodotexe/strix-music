using System;

namespace StrixMusic.Sdk.Models.Base
{
    /// <summary>
    /// Contains recently played albums, artists, tracks, playlists, etc.
    /// </summary>
    public interface IRecentlyPlayedBase : IPlayableCollectionGroupBase, IAsyncDisposable
    {
    }
}