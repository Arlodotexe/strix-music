using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Interfaces
{
    /// <summary>
    /// A collection of artists.
    /// </summary>
    public interface IArtistCollection : IPlayableCollectionBase
    {
        /// <summary>
        /// The artists in the collection.
        /// </summary>
        ObservableCollection<IArtist> Artists { get; }

        /// <summary>
        /// The total number of available <see cref="Artists"/>.
        /// </summary>
        int TotalArtistsCount { get; }

        /// <summary>
        /// Checks if the backend supports adding an <see cref="IArtist"/> at a specific position in <see cref="Artists"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, a new <see cref="IArtist"/> can be added.</returns>
        Task<bool> IsAddArtistSupported(int index);

        /// <summary>
        /// A collection that maps (by index) to the items in <see cref="Artists"/>. The bool at each index tells you if removing the <see cref="IArtist"/> is supported.
        /// </summary>
        ObservableCollection<bool> IsRemoveArtistSupportedMap { get; }

        /// <summary>
        /// Returns items at a specific index and offset.
        /// </summary>
        /// <remarks>Does not affect <see cref="Artists"/>.</remarks>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        IAsyncEnumerable<IArtist> GetArtistsAsync(int limit, int offset);

        /// <summary>
        /// Populates more items into <see cref="Artists"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PopulateMoreArtistsAsync(int limit);
    }
}
