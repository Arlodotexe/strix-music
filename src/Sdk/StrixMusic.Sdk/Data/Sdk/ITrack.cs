using System;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Core.Data
{
    /// <inheritdoc cref="ITrackBase"/>
    /// <remarks>This interface should be implemented by the Sdk.</remarks>
    public interface ITrack : ITrackBase, IArtistCollection, IGenreCollection, ISdkMember
    {
        /// <summary>
        /// An <see cref="IAlbum"/> object that this track belongs to.
        /// </summary>
        IAlbum? Album { get; }

        /// <summary>
        /// The lyrics for this track.
        /// </summary>
        ILyrics? Lyrics { get; }

        /// <summary>
        /// A <see cref="IPlayable"/> of items related to this item.
        /// </summary>
        IPlayableCollectionGroup? RelatedItems { get; }

        /// <summary>
        /// Changes the <see cref="Lyrics"/> for this track.
        /// </summary>
        /// <param name="lyrics">The new lyrics data.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ChangeLyricsAsync(ILyrics? lyrics);

        /// <summary>
        /// Fires when the <see cref="Album"/> metadata changes.
        /// </summary>
        event EventHandler<IAlbum?> AlbumChanged;

        /// <summary>
        /// Fires when the <see cref="Lyrics"/> metadata changes.
        /// </summary>
        event EventHandler<ILyrics?> LyricsChanged;

        /// <summary>
        /// Changes the album for this track.
        /// </summary>
        /// <param name="albums">The new album.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ChangeAlbumAsync(IAlbum? albums);
    }
}