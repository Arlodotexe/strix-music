using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.Input;
using StrixMusic.Sdk.Models;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// An interfaced ViewModel for <see cref="IUrlCollection" />. This is needed so because multiple view models implement <see cref="IUrlCollection"/>, and the UI needs to create controls that handle only the ViewModels properties for an <see cref="IUrlCollection"/>.
    /// </summary>
    public interface IUrlCollectionViewModel : IUrlCollection
    {
        /// <summary>
        /// The urls in this collection.
        /// </summary>
        public ObservableCollection<IUrl> Urls { get; }

        /// <summary>
        /// Populates the next set of urls into the collection.
        /// </summary>
        /// <param name="limit">The number of items to load.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task PopulateMoreUrlsAsync(int limit);

        /// <summary>
        /// <inheritdoc cref="PopulateMoreUrlsAsync"/>
        /// </summary>
        public IAsyncRelayCommand<int> PopulateMoreUrlsCommand { get; }
    }
}