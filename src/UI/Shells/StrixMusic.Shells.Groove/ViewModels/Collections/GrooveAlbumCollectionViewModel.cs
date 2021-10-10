using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.Groove.Messages.Navigation.Pages;
using System;

namespace StrixMusic.Shells.Groove.ViewModels.Collections
{
    public class GrooveAlbumCollectionViewModel : ObservableObject
    {
        private IAlbumCollectionViewModel? _albumCollectionViewModel;

        public GrooveAlbumCollectionViewModel() 
            : this(null)
        {
        }

        public GrooveAlbumCollectionViewModel(IAlbumCollectionViewModel? viewModel)
        {
            NavigateToAlbumCommand = new RelayCommand<AlbumViewModel>(NavigateToAlbum);
        }

        public event EventHandler? ViewModelSet;

        public IAlbumCollectionViewModel? ViewModel
        {
            get => _albumCollectionViewModel;
            set
            {
                SetProperty(ref _albumCollectionViewModel, value);
                ViewModelSet?.Invoke(this, EventArgs.Empty);

#warning Bind to INPC with Behaviors
                ViewModel?.InitAlbumCollectionAsync();
            }
        }

        public RelayCommand<AlbumViewModel> NavigateToAlbumCommand { get; private set; }

        private void NavigateToAlbum(AlbumViewModel? viewModel)
        {
            if (viewModel != null)
                WeakReferenceMessenger.Default.Send(new AlbumViewNavigationRequested(viewModel));
        }
    }
}
