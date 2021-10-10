using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.Groove.Messages.Navigation.Pages;

namespace StrixMusic.Shells.Groove.ViewModels.Collections
{
    /// <summary>
    /// A ViewModel for a <see cref="Controls.Collections.GrooveAlbumCollection"/>.
    /// </summary>
    public class GrooveAlbumCollectionViewModel : ObservableObject
    {
        private IAlbumCollectionViewModel? _albumCollectionViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="GrooveAlbumCollectionViewModel"/> class.
        /// </summary>
        public GrooveAlbumCollectionViewModel(IAlbumCollectionViewModel viewModel)
        {
            ViewModel = viewModel;
            NavigateToAlbumCommand = new RelayCommand<AlbumViewModel>(NavigateToAlbum);
        }

        /// <summary>
        /// The <see cref="IAlbumCollectionViewModel"/> inside this ViewModel on display.
        /// </summary>
        public IAlbumCollectionViewModel? ViewModel
        {
            get => _albumCollectionViewModel;
            set => SetProperty(ref _albumCollectionViewModel, value);
        }

        /// <summary>
        /// A Command that requests a navigation to an album page.
        /// </summary>
        public RelayCommand<AlbumViewModel> NavigateToAlbumCommand { get; private set; }

        private void NavigateToAlbum(AlbumViewModel? viewModel)
        {
            if (viewModel != null)
                WeakReferenceMessenger.Default.Send(new AlbumViewNavigationRequested(viewModel));
        }
    }
}
