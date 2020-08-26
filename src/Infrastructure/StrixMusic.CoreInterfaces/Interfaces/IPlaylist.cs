namespace StrixMusic.CoreInterfaces.Interfaces
{
    /// <summary>
    /// Interface that represents a playlist.
    /// </summary>
    public interface IPlaylist : ITrackCollection
    {
        /// <summary>
        /// Suggested tracks that the user may want to add to this playlist.
        /// </summary>
        ITrackCollection SuggestedTracks { get; }
    }
}
