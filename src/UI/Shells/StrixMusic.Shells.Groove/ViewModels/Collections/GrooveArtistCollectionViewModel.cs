using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.Groove.Messages.Navigation.Pages;
using System;

namespace StrixMusic.Shells.Groove.ViewModels.Collections
{
    public class GrooveArtistCollectionViewModel : ObservableObject
    {
        private IArtistCollectionViewModel? _artistCollectionViewModel;

        public GrooveArtistCollectionViewModel()
            : this(null)
        {
        }

        public GrooveArtistCollectionViewModel(IArtistCollectionViewModel? viewModel)
        {
            NavigateToArtistCommand = new RelayCommand<ArtistViewModel>(NavigateToArtist);
        }

        public event EventHandler? ViewModelSet;

        public IArtistCollectionViewModel? ViewModel
        {
            get => _artistCollectionViewModel;
            set
            {
                SetProperty(ref _artistCollectionViewModel, value);
                ViewModelSet?.Invoke(this, EventArgs.Empty);
            }
        }

        public RelayCommand<ArtistViewModel> NavigateToArtistCommand { get; private set; }

        private void NavigateToArtist(ArtistViewModel? viewModel)
        {
            if (viewModel != null)
                WeakReferenceMessenger.Default.Send(new ArtistViewNavigationRequested(viewModel));
        }
    }
}
