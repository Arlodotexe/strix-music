using System;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Models.Base
{
    /// <summary>
    /// A common interface for all collections that return urls.
    /// </summary>
    public interface IUrlCollectionBase : ICollectionBase, IAsyncDisposable
    {
        /// <summary>
        /// The total number of urls in this collection.
        /// </summary>
        int TotalUrlCount { get; }

        /// <summary>
        /// Removes a url from the collection.
        /// </summary>
        /// <param name="index">the position remove the url from.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task RemoveUrlAsync(int index);

        /// <summary>
        /// Checks if adding a url to the collection at at the given <paramref name="index"/> is supported.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, a new item can be added to the collection.</returns>
        Task<bool> IsAddUrlAvailableAsync(int index);

        /// <summary>
        /// Checks if removing a url to the collection at at the given <paramref name="index"/> is supported.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, the item can be removed from the collection..</returns>
        Task<bool> IsRemoveUrlAvailableAsync(int index);

        /// <summary>
        /// Fires when the merged number of urls in the collection changes.
        /// </summary>
        event EventHandler<int>? UrlsCountChanged;
    }
}