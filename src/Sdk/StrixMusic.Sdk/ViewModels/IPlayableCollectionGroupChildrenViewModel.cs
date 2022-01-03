using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.Input;
using StrixMusic.Sdk.Models;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// The children-specific ViewModel properties for <see cref="IPlayableCollectionGroup"/>. This is needed so because multiple view models implement <see cref="IPlayableCollectionGroup"/>, and the UI needs to create controls that handle only the ViewModels properties for an <see cref="IPlayableCollectionGroup"/>.
    /// </summary>
    public interface IPlayableCollectionGroupChildrenViewModel : IPlayableCollectionGroupChildren
    {
        /// <summary>
        /// The nested <see cref="IPlayableCollectionGroup"/> items in this collection.
        /// </summary>
        public ObservableCollection<PlayableCollectionGroupViewModel> Children { get; }

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