namespace StrixMusic.CoreInterfaces.Interfaces
{
    /// <summary>
    /// Interface representing an album.
    /// </summary>
    public interface IAlbum : IPlayableCollection
    {
        /// <summary>
        /// An <see cref="IArtist"/> object that this album was created by.
        /// </summary>
        IArtist Artist { get; }
    }
}
