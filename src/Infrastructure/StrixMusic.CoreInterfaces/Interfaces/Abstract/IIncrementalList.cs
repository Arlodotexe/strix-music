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
    public interface IIncrementalList<T>
    {
        /// <summary>
        /// Loads the next set of Items.
        /// </summary>
        /// <returns>A <see cref="IAsyncEnumerable"/> for loading the next page.</returns>
        IAsyncEnumerable<T> LoadFurtherAsync();
    }
}
