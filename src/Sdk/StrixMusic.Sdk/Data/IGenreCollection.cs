using OwlCore.Events;
using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Data.Merged;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Data
{
    /// <inheritdoc cref="IGenreCollectionBase" />
    /// <remarks>This interface should be implemented by the Sdk.</remarks>
    public interface IGenreCollection : IGenreCollectionBase, ISdkMember, IMerged<ICoreGenreCollection>
    {
        /// <summary>
        /// Adds a new genre to the collection.
        /// </summary>
        /// <param name="genre">The genre to insert.</param>
        /// <param name="index">the position to insert the genre at.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task AddGenreAsync(IGenre genre, int index);

        /// <summary>
        /// Fires when the genres are changed.
        /// </summary>
        event CollectionChangedEventHandler<IGenre>? GenresChanged;
    }
}