// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.Input;
using StrixMusic.Sdk.Models;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// An interfaced ViewModel for <see cref="IGenreCollection" />.
    /// This is needed so because multiple view models implement <see cref="IGenreCollection"/>,
    /// and the UI needs to create controls that handle only the ViewModels properties for an <see cref="IGenreCollection"/>.
    /// </summary>
    public interface IGenreCollectionViewModel : ISdkViewModel, IGenreCollection
    {
        /// <summary>
        /// The genres in this collection.
        /// </summary>
        public ObservableCollection<IGenre> Genres { get; }

        /// <summary>
        /// Populates the next set of genres into the collection.
        /// </summary>
        /// <param name="limit">The number of items to load.</param>
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task PopulateMoreGenresAsync(int limit, CancellationToken cancellationToken = default);

        /// <summary>
        /// Loads the entire collection of <see cref="IGenre"/>s and ensures all sources are merged.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task InitGenreCollectionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Initializes the list of the <see cref="IGenre"/>.
        /// </summary>
        public IAsyncRelayCommand InitGenreCollectionAsyncCommand { get; }

        /// <summary>
        /// <inheritdoc cref="PopulateMoreGenresAsync"/>
        /// </summary>
        public IAsyncRelayCommand<int> PopulateMoreGenresCommand { get; }
    }
}
