using System;
using System.Threading.Tasks;
using StrixMusic.Sdk.Data.Base;

namespace StrixMusic.Sdk.Data.Core
{
    /// <inheritdoc cref="ITrackBase"/>
    /// <remarks>This interface should be implemented by a core.</remarks>
    public interface ICoreTrack : ITrackBase, ICoreArtistCollection, ICoreGenreCollection, ICoreMember
    {
        /// <summary>
        /// An <see cref="ICoreAlbum"/> object that this track belongs to.
        /// </summary>
        ICoreAlbum? Album { get; }

        /// <summary>
        /// The lyrics for this track.
        /// </summary>
        ICoreLyrics? Lyrics { get; }

        /// <summary>
        /// A <see cref="IPlayable"/> of items related to this item.
        /// </summary>
        ICorePlayableCollectionGroup? RelatedItems { get; }

        /// <summary>
        /// Changes the <see cref="Lyrics"/> for this track.
        /// </summary>
        /// <param name="lyrics">The new lyrics data.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ChangeLyricsAsync(ICoreLyrics? lyrics);

        /// <summary>
        /// Fires when the <see cref="Album"/> metadata changes.
        /// </summary>
        event EventHandler<ICoreAlbum?> AlbumChanged;

        /// <summary>
        /// Fires when the <see cref="Lyrics"/> metadata changes.
        /// </summary>
        event EventHandler<ICoreLyrics?> LyricsChanged;

        /// <summary>
        /// Changes the album for this track.
        /// </summary>
        /// <param name="albums">The new album.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ChangeAlbumAsync(ICoreAlbum? albums);
    }
}
