using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.Input;
using OwlCore.Collections;
using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// An observable <see cref="IPlayableCollectionGroupBase"/>.
    /// </summary>
    public interface IPlayableCollectionGroupChildrenViewModel : ICorePlayableCollectionGroupChildren
    {
        /// <summary>
        /// The nested <see cref="IPlayableCollectionGroupBase"/> items in this collection.
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