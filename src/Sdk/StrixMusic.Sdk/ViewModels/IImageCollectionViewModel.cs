// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using OwlCore.Provisos;
using StrixMusic.Sdk.Models;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// An interfaced ViewModel for <see cref="IImageCollection" />.
    /// This is needed so because multiple view models implement <see cref="IImageCollection"/>,
    /// and the UI needs to create controls that handle only the ViewModels properties for an <see cref="IImageCollection"/>.
    /// </summary>
    public interface IImageCollectionViewModel : ISdkViewModel, IImageCollection, IAsyncInit
    {
        /// <summary>
        /// The images in this collection.
        /// </summary>
        public ObservableCollection<IImage> Images { get; }

        /// <summary>
        /// Populates the next set of images into the collection.
        /// </summary>
        /// <param name="limit">The number of items to load.</param>
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task PopulateMoreImagesAsync(int limit, CancellationToken cancellationToken = default);

        /// <summary>
        /// <inheritdoc cref="PopulateMoreImagesAsync"/>
        /// </summary>
        public IAsyncRelayCommand<int> PopulateMoreImagesCommand { get; }

        /// <summary>
        /// Loads the entire collection of <see cref="IImage"/>s and ensures all sources are merged.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task InitImageCollectionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Initializes the list of the <see cref="IImage"/>.
        /// </summary>
        public IAsyncRelayCommand InitImageCollectionAsyncCommand { get; }
    }
}
