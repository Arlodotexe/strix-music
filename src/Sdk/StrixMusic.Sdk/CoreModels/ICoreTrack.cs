// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;
using StrixMusic.Sdk.BaseModels;

namespace StrixMusic.Sdk.CoreModels
{
    /// <summary>
    /// Represents an audio stream with metadata that belongs to an <see cref="ICoreTrackCollection"/>.
    /// </summary>
    /// <remarks>This interface should be implemented by a core.</remarks>
    public interface ICoreTrack : ITrackBase, ICoreArtistCollection, ICoreGenreCollection, ICoreModel
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
        /// A <see cref="IPlayableBase"/> of items related to this item.
        /// </summary>
        ICorePlayableCollectionGroup? RelatedItems { get; }

        /// <summary>
        /// Changes the <see cref="Lyrics"/> for this track.
        /// </summary>
        /// <param name="lyrics">The new lyrics data.</param>
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ChangeLyricsAsync(ICoreLyrics? lyrics, CancellationToken cancellationToken = default);

        /// <summary>
        /// Fires when the <see cref="Album"/> metadata changes.
        /// </summary>
        event EventHandler<ICoreAlbum?>? AlbumChanged;

        /// <summary>
        /// Fires when the <see cref="Lyrics"/> metadata changes.
        /// </summary>
        event EventHandler<ICoreLyrics?>? LyricsChanged;

        /// <summary>
        /// Changes the album for this track.
        /// </summary>
        /// <param name="albums">The new album.</param>
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ChangeAlbumAsync(ICoreAlbum? albums, CancellationToken cancellationToken = default);
    }
}
