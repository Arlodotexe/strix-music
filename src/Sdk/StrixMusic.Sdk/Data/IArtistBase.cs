namespace StrixMusic.Sdk.Core.Data
{
    /// <summary>
    /// A musician or creator that has published one or more <see cref="ITrack"/>s and <see cref="IAlbum"/>s.
    /// </summary>
    public interface IArtistBase : IPlayable, IArtistCollectionItemBase, IAlbumCollectionBase, ITrackCollectionBase, IGenreCollectionBase
    {
    }
}