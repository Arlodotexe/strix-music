using System;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Data.Base
{
    /// <summary>
    /// Interface representing an album.
    /// </summary>
    public interface IAlbumBase : IPlayableCollectionItem, IAlbumCollectionItemBase, IArtistCollectionBase, ITrackCollectionBase, IImageCollectionBase, IGenreCollectionBase
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
        /// Fires when the <see cref="DatePublished"/> metadata changes.
        /// </summary>
        event EventHandler<DateTime?>? DatePublishedChanged;
    }
}