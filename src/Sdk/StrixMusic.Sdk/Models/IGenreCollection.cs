using OwlCore.Events;
using StrixMusic.Sdk.Models.Base;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Models.Merged;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Models
{
    /// <inheritdoc cref="IGenreCollectionBase" />
    /// <remarks>This interface should be implemented by the Sdk.</remarks>
    public interface IGenreCollection : IGenreCollectionBase, ISdkMember, IMerged<ICoreGenreCollection>
    {
        /// <summary>
        /// Gets a requested number of <see cref="IGenreBase"/>s starting at the given offset.
        /// </summary>
        /// <param name="limit">The max number of items to return.</param>
        /// <param name="offset">Get items starting at this index.</param>
        /// <returns><see cref="IReadOnlyList{T}"/> containing the requested items.</returns>
        Task<IReadOnlyList<IGenre>> GetGenresAsync(int limit, int offset);

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