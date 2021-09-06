using OwlCore.Events;
using StrixMusic.Sdk.Data.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Data.Core
{
    /// <inheritdoc cref="IUrlCollectionBase" />
    /// <remarks>This interface should be implemented by a core.</remarks>
    public interface ICoreUrlCollection : ICoreCollection, IUrlCollectionBase, ICoreMember
    {
        /// <summary>
        /// Gets a requested number of <see cref="ICoreUrl"/>s starting at the given offset in the backend.
        /// </summary>
        /// <param name="limit">The max number of items to return.</param>
        /// <param name="offset">Get items starting at this index.</param>
        /// <returns><see cref="IAsyncEnumerable{T}"/> containing the requested items.</returns>
        IAsyncEnumerable<ICoreUrl> GetUrlsAsync(int limit, int offset);

        /// <summary>
        /// Adds a new url to the collection.
        /// </summary>
        /// <param name="url">The url to insert.</param>
        /// <param name="index">the position to insert the url at.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task AddUrlAsync(ICoreUrl url, int index);

        /// <summary>
        /// Fires when the urls are changed.
        /// </summary>
        event CollectionChangedEventHandler<ICoreUrl>? UrlsChanged;
    }
}
