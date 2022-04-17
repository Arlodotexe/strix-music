// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.BaseModels
{
    /// <summary>
    /// A published album containing one or more tracks, discs, artist, etc.
    /// </summary>
    public interface IAlbumBase : IPlayableCollectionItem, IAlbumCollectionItemBase, IArtistCollectionBase, ITrackCollectionBase, IImageCollectionBase, IGenreCollectionBase, IAsyncDisposable
    {
        /// <summary>
        /// The date the album was released.
        /// </summary>
        DateTime? DatePublished { get; }

        /// <summary>
        /// If true, <see cref="ChangeDatePublishedAsync"/> is supported.
        /// </summary>
        bool IsChangeDatePublishedAsyncAvailable { get; }

        /// <summary>
        /// Changes the <see cref="DatePublished"/> for this album.
        /// </summary>
        /// <param name="datePublished">The new date the album was published.</param>
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ChangeDatePublishedAsync(DateTime datePublished, CancellationToken cancellationToken = default);

        /// <summary>
        /// Raised when <see cref="DatePublished"/> is changed.
        /// </summary>
        event EventHandler<DateTime?>? DatePublishedChanged;

        /// <summary>
        /// Raised when <see cref="IsChangeDatePublishedAsyncAvailable"/> is changed.
        /// </summary>
        event EventHandler<bool>? IsChangeDatePublishedAsyncAvailableChanged;
    }
}
