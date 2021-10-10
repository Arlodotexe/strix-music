using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.Groove.Messages.Navigation.Pages;
using System;

namespace StrixMusic.Shells.Groove.ViewModels.Collections
{
    public class GroovePlaylistCollectionViewModel : ObservableObject
    {
        private IPlaylistCollectionViewModel? _playlistCollectionViewModel;

        public GroovePlaylistCollectionViewModel()
            : this(null)
        {
        }

        public GroovePlaylistCollectionViewModel(IPlaylistCollectionViewModel? viewModel)
        {
            NavigateToPlaylistCommand = new RelayCommand<PlaylistViewModel>(NavigateToPlaylist);
        }

        public event EventHandler? ViewModelSet;

        public IPlaylistCollectionViewModel? ViewModel
        {
            get => _playlistCollectionViewModel;
            set
            {
                SetProperty(ref _playlistCollectionViewModel, value);
                ViewModelSet?.Invoke(this, EventArgs.Empty);
            }
        }

        public RelayCommand<PlaylistViewModel> NavigateToPlaylistCommand { get; private set; }

        private void NavigateToPlaylist(PlaylistViewModel? viewModel)
        {
            if (viewModel != null)
                WeakReferenceMessenger.Default.Send(new PlaylistViewNavigationRequested(viewModel));
        }
    }
}
