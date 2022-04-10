// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Toolkit.Mvvm.Input;
using StrixMusic.Sdk.Models;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// The children-specific ViewModel properties for <see cref="IPlayableCollectionGroup"/>.
    /// This is needed so because multiple view models implement <see cref="IPlayableCollectionGroup"/>,
    /// and the UI needs to create controls that handle only the ViewModels properties for an <see cref="IPlayableCollectionGroup"/>.
    /// </summary>
    public interface IPlayableCollectionGroupChildrenViewModel : ISdkViewModel, IPlayableCollectionGroupChildren
    {
        /// <summary>
        /// The nested <see cref="IPlayableCollectionGroup"/> items in this collection.
        /// </summary>
        public ObservableCollection<PlayableCollectionGroupViewModel> Children { get; }

        /// <summary>
        /// Populates the next set of children items into the collection.
        /// </summary>
        /// <param name="limit">The number of items to load.</param>
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task PopulateMoreChildrenAsync(int limit, CancellationToken cancellationToken = default);

        /// <summary>
        /// <inheritdoc cref="PopulateMoreChildrenAsync"/>
        /// </summary>
        public IAsyncRelayCommand<int> PopulateMoreChildrenCommand { get; }
    }
}
