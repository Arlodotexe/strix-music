using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.Input;
using OwlCore.Provisos;
using StrixMusic.Sdk.Data;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// An interfaced ViewModel for <see cref="IImageCollection" />. This is needed so because multiple view models implement <see cref="IImageCollection"/>, and the UI needs to create controls that handle only the ViewModels properties for an <see cref="IImageCollection"/>.
    /// </summary>
    public interface IImageCollectionViewModel : IImageCollection, IAsyncInit
    {
        /// <summary>
        /// The images in this collection.
        /// </summary>
        public ObservableCollection<IImage> Images { get; }

        /// <summary>
        /// Populates the next set of images into the collection.
        /// </summary>
        /// <param name="limit">The number of items to load.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task PopulateMoreImagesAsync(int limit);

        /// <summary>
        /// <inheritdoc cref="PopulateMoreImagesAsync"/>
        /// </summary>
        public IAsyncRelayCommand<int> PopulateMoreImagesCommand { get; }

        /// <summary>
        /// Loads the entire collection of <see cref="IImage"/>s and ensures all sources are merged.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task InitImageCollectionAsync();

        /// <summary>
        /// Initializes the list of the <see cref="IImage"/>.
        /// </summary>
        public IAsyncRelayCommand InitImageCollectionAsyncCommand { get; }
    }
}