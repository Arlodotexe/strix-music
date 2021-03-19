using System;

namespace StrixMusic.Sdk.Data.Base
{
    /// <summary>
    /// A musician or creator that has published one or more <see cref="ITrack"/>s and <see cref="IAlbum"/>s.
    /// </summary>
    public interface IArtistBase : IPlayableCollectionItem, IArtistCollectionItemBase, IAlbumCollectionBase, ITrackCollectionBase, IGenreCollectionBase, IAsyncDisposable
    {
    }
}