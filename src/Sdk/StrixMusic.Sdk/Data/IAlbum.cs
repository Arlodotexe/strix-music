using System;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Core.Data
{
    /// <summary>
    /// Interface representing an album.
    /// </summary>
    public interface IAlbum : IPlayable, IAlbumCollectionItem, ITrackCollectionBase, IGenreCollection
    {
        /// <summary>
        /// An <see cref="ICoreArtist"/> object that this album was created by.
        /// </summary>
        ICoreArtist CoreArtist { get; }

        /// <summary>
        /// A <see cref="IPlayable"/> of items related to this item.
        /// </summary>
        IPlayableCollectionGroup? RelatedItems { get; }

        /// <summary>
        /// The date the album was released.
        /// </summary>
        DateTime? DatePublished { get; }

        /// <summary>
        /// If true, <see cref="ChangeDatePublishedAsync(DateTime)"/> is supported.
        /// </summary>
        bool IsChangeDatePublishedAsyncSupported { get; }

        /// <summary>
        /// Changes the <see cref="DatePublished"/> for this album.
        /// </summary>
        /// <param name="datePublished">The new date the album was published.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ChangeDatePublishedAsync(DateTime datePublished);

        /// <summary>
        /// Fires when the <see cref="DatePublished"/> metadata changes.
        /// </summary>
        event EventHandler<DateTime?> DatePublishedChanged;
    }
}