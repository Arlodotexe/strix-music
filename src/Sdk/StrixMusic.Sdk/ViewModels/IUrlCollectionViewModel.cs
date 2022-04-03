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
    /// An interfaced ViewModel for <see cref="IUrlCollection" />.
    /// This is needed so because multiple view models implement <see cref="IUrlCollection"/>,
    /// and the UI needs to create controls that handle only the ViewModels properties for an <see cref="IUrlCollection"/>.
    /// </summary>
    public interface IUrlCollectionViewModel : ISdkViewModel, IUrlCollection
    {
        /// <summary>
        /// The urls in this collection.
        /// </summary>
        public ObservableCollection<IUrl> Urls { get; }

        /// <summary>
        /// Populates the next set of urls into the collection.
        /// </summary>
        /// <param name="limit">The number of items to load.</param>
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task PopulateMoreUrlsAsync(int limit, CancellationToken cancellationToken = default);

        /// <summary>
        /// <inheritdoc cref="PopulateMoreUrlsAsync"/>
        /// </summary>
        public IAsyncRelayCommand<int> PopulateMoreUrlsCommand { get; }
    }
}
