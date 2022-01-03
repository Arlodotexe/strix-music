using OwlCore.Events;
using StrixMusic.Sdk.Models.Base;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Models.Merged;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Models
{
    /// <inheritdoc cref="IUrlCollectionBase" />
    /// <remarks>This interface should be implemented by the Sdk.</remarks>
    public interface IUrlCollection : IUrlCollectionBase, ISdkMember, IMerged<ICoreUrlCollection>
    {
        /// <summary>
        /// Gets a requested number of <see cref="IUrlBase"/>s starting at the given offset.
        /// </summary>
        /// <param name="limit">The max number of items to return.</param>
        /// <param name="offset">Get items starting at this index.</param>
        /// <returns><see cref="IReadOnlyList{T}"/> containing the requested items.</returns>
        Task<IReadOnlyList<IUrl>> GetUrlsAsync(int limit, int offset);

        /// <summary>
        /// Adds a new url to the collection.
        /// </summary>
        /// <param name="url">The url to insert.</param>
        /// <param name="index">the position to insert the url at.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task AddUrlAsync(IUrl url, int index);

        /// <summary>
        /// Fires when the urls are changed.
        /// </summary>
        event CollectionChangedEventHandler<IUrl>? UrlsChanged;
    }
}