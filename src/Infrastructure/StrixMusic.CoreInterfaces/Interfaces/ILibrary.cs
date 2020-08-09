namespace StrixMusic.CoreInterfaces.Interfaces
{
    /// <summary>
    /// Contains a music library.
    /// </summary>
    public interface ILibrary : IArtistCollection, IAlbumCollection, ITrackCollection, IPlaylistCollection
    {
    }
}
