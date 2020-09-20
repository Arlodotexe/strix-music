using System;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Interfaces
{
    /// <summary>
    /// Interface representing an album.
    /// </summary>
    public interface IAlbum : ITrackCollection, IGenreCollection
    {
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

        /// <summary>
        /// An <see cref="IArtist"/> object that this album was created by.
        /// </summary>
        IArtist Artist { get; }

        /// <summary>
        /// A <see cref="IPlayable"/> of items related to this item.
        /// </summary>
        IPlayableCollectionGroup? RelatedItems { get; }
    }
}
