using OwlCore.Events;
using StrixMusic.Sdk.Data.Base;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Data.Core
{
    /// <inheritdoc cref="IGenreCollectionBase" />
    /// <remarks>This interface should be implemented by a core.</remarks>
    public interface ICoreGenreCollection : ICoreCollection, IGenreCollectionBase, ICoreMember
    {
        /// <summary>
        /// Adds a new genre to the collection.
        /// </summary>
        /// <param name="genre">The genre to insert.</param>
        /// <param name="index">the position to insert the genre at.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task AddGenreAsync(ICoreGenre genre, int index);

        /// <summary>
        /// Removes a genre from the collection.
        /// </summary>
        /// <param name="genre">The genre to remove.</param>
        /// <param name="index">the position remove the genre from.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task RemoveGenreAsync(ICoreGenre genre, int index);

        /// <summary>
        /// Fires when the genres are changed.
        /// </summary>
        event CollectionChangedEventHandler<ICoreGenre>? GenresChanged;
    }
}
