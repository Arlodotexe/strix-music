namespace StrixMusic.CoreInterfaces.Interfaces
{
    /// <summary>
    /// Contains recently played items.
    /// </summary>
    public interface IRecentlyPlayed : IArtistCollection, IAlbumCollection, ITrackCollection, IPlaylistCollection
    {
    }
}
