// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OwlCore.Events;
using StrixMusic.Sdk.AdapterModels;
using StrixMusic.Sdk.BaseModels;
using StrixMusic.Sdk.CoreModels;

namespace StrixMusic.Sdk.AppModels
{
    /// <summary>
    /// A collection of <see cref="IGenre"/>s.
    /// </summary>
    /// <remarks>Instances of this class may contain data merged from one or more sources.</remarks>
    public interface IGenreCollection : IGenreCollectionBase, ISdkMember, IMerged<ICoreGenreCollection>
    {
        /// <summary>
        /// Gets a requested number of <see cref="IGenreBase"/>s starting at the given offset.
        /// </summary>
        /// <param name="limit">The max number of items to return.</param>
        /// <param name="offset">Get items starting at this index.</param>
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns><see cref="IReadOnlyList{T}"/> containing the requested items.</returns>
        Task<IReadOnlyList<IGenre>> GetGenresAsync(int limit, int offset, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a new genre to the collection.
        /// </summary>
        /// <param name="genre">The genre to insert.</param>
        /// <param name="index">the position to insert the genre at.</param>
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task AddGenreAsync(IGenre genre, int index, CancellationToken cancellationToken = default);

        /// <summary>
        /// Fires when the genres are changed.
        /// </summary>
        event CollectionChangedEventHandler<IGenre>? GenresChanged;
    }
}
