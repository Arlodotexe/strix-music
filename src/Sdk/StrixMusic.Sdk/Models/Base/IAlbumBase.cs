using System;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Models.Base
{
    /// <summary>
    /// Interface representing an album.
    /// </summary>
    public interface IAlbumBase : IPlayableCollectionItem, IAlbumCollectionItemBase, IArtistCollectionBase, ITrackCollectionBase, IImageCollectionBase, IGenreCollectionBase, IAsyncDisposable
    {
        /// <summary>
        /// The date the album was released.
        /// </summary>
        DateTime? DatePublished { get; }

        /// <summary>
        /// If true, <see cref="ChangeDatePublishedAsync(DateTime)"/> is supported.
        /// </summary>
        bool IsChangeDatePublishedAsyncAvailable { get; }

        /// <summary>
        /// Changes the <see cref="DatePublished"/> for this album.
        /// </summary>
        /// <param name="datePublished">The new date the album was published.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ChangeDatePublishedAsync(DateTime datePublished);

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