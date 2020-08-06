using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace StrixMusic.CoreInterfaces.Interfaces.Abstract
{
    /// <summary>
    /// A paginated list of <typeparamref name="T"/> that can be loaded by ViewModel.
    /// </summary>
    /// <remarks>
    /// The paginated list doesn't need a <see cref="Mutex"/>, locking is handled in the ViewModel.
    /// </remarks>
    /// <typeparam name="T">The type of items contained in the list.</typeparam>
    public interface IPaginatedList<T>
    {
        /// <summary>
        /// Loads the next page and returns its contents
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation loading the next page.</returns>
        Task<IEnumerable<T>> LoadNextPage();

        /// <summary>
        /// Loads the previous page and returns its contents
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation loading the previous page.</returns>
        Task<IEnumerable<T>> LoadPreviousPage();
    }
}
