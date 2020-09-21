using System.Threading.Tasks;
using OwlCore.Collections;

namespace StrixMusic.Sdk.Interfaces
{
    /// <summary>
    /// Metadata about genres.
    /// </summary>
    public interface IGenreCollection
    {
        /// <summary>
        /// A list of <see cref="string"/> describing the genres for this track.
        /// </summary>
        SynchronizedObservableCollection<string>? Genres { get; }

        /// <summary>
        /// Checks if the backend supports adding a <see cref="string"/> at a specific position in <see cref="Genres"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, a new <see cref="string"/> can be added.</returns>
        Task<bool> IsAddGenreSupported(int index);

        /// <summary>
        /// A collection that maps (by index) to the items in <see cref="Genres"/>. The bool at each index tells you if removing the <see cref="string"/> is supported.
        /// </summary>
        SynchronizedObservableCollection<bool> IsRemoveGenreSupportedMap { get; }
    }
}