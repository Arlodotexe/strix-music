using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.Groove.Messages.Navigation.Pages;

namespace StrixMusic.Shells.Groove.ViewModels.Collections
{
    /// <summary>
    /// A ViewModel for a <see cref="Controls.Collections.GrooveArtistCollection"/>.
    /// </summary>
    public class GrooveArtistCollectionViewModel : ObservableObject
    {
        private IArtistCollectionViewModel? _artistCollectionViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="GrooveArtistCollectionViewModel"/> class.
        /// </summary>
        public GrooveArtistCollectionViewModel(IArtistCollectionViewModel? viewModel)
        {
            ViewModel = viewModel;
            NavigateToArtistCommand = new RelayCommand<ArtistViewModel>(NavigateToArtist);
        }

        /// <summary>
        /// The <see cref="IArtistCollectionViewModel"/> inside this ViewModel on display.
        /// </summary>
        public IArtistCollectionViewModel? ViewModel
        {
            get => _artistCollectionViewModel;
            set => SetProperty(ref _artistCollectionViewModel, value);
        }

        /// <summary>
        /// A Command that requests a navigation to an artist page.
        /// </summary>
        public RelayCommand<ArtistViewModel> NavigateToArtistCommand { get; private set; }

        private void NavigateToArtist(ArtistViewModel? viewModel)
        {
            if (viewModel != null)
                WeakReferenceMessenger.Default.Send(new ArtistViewNavigationRequestMessage(viewModel));
        }
    }
}
