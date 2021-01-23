using System.Threading.Tasks;
using OwlCore.Collections;

namespace StrixMusic.Sdk.Data.Base
{
    /// <summary>
    /// Metadata about genres.
    /// </summary>
    public interface IGenreCollectionBase
    {
        /// <summary>
        /// A list of <see cref="string"/> describing the genres for this track.
        /// </summary>
        /// <remarks>Data should be populated on object creation. Handle <see cref="SynchronizedObservableCollection{T}.CollectionChanged"/> to find out when a genre is added or removed.</remarks>
        SynchronizedObservableCollection<string>? Genres { get; }

        /// <summary>
        /// Checks if the backend supports adding a <see cref="string"/> at a specific position in <see cref="Genres"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, a new <see cref="string"/> can be added.</returns>
        Task<bool> IsAddGenreAvailable(int index);

        /// <summary>
        /// Checks if the backend supports removing a <see cref="string"/> at a specific index.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, the <see cref="string"/> can be removed.</returns>
        Task<bool> IsRemoveGenreAvailable(int index);
    }
}