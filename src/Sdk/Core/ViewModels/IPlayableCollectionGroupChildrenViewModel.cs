using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.Input;
using OwlCore.Collections;
using StrixMusic.Sdk.Core.Data;

namespace StrixMusic.Sdk.Core.ViewModels
{
    /// <summary>
    /// An observable <see cref="IPlayableCollectionGroup"/>.
    /// </summary>
    public interface IPlayableCollectionGroupChildrenViewModel : IPlayableCollectionGroupChildren
    {
        /// <summary>
        /// The nested <see cref="IPlayableCollectionGroup"/> items in this collection.
        /// </summary>
        public SynchronizedObservableCollection<PlayableCollectionGroupViewModel> Children { get; }

        /// <summary>
        /// Populates the next set of children items into the collection.
        /// </summary>
        /// <param name="limit">The number of items to load.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task PopulateMoreChildrenAsync(int limit);

        /// <summary>
        /// <inheritdoc cref="PopulateMoreChildrenAsync"/>
        /// </summary>
        public IAsyncRelayCommand<int> PopulateMoreChildrenCommand { get; }
    }
}