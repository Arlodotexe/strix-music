using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.Groove.Messages.Navigation.Pages;
using System;

namespace StrixMusic.Shells.Groove.ViewModels.Collections
{
    public class GrooveTrackCollectionViewModel : ObservableObject
    {
        private ITrackCollectionViewModel? _trackCollectionViewModel;

        public GrooveTrackCollectionViewModel()
            : this(null)
        {
        }

        public GrooveTrackCollectionViewModel(ITrackCollectionViewModel? viewModel)
        {
            NavigateToTrackCommand = new RelayCommand<TrackViewModel>(NavigateToTrack);
        }

        public event EventHandler? ViewModelSet;

        public ITrackCollectionViewModel? ViewModel
        {
            get => _trackCollectionViewModel;
            set
            {
                SetProperty(ref _trackCollectionViewModel, value);
                ViewModelSet?.Invoke(this, EventArgs.Empty);
            }
        }

        public RelayCommand<TrackViewModel> NavigateToTrackCommand { get; private set; }

        private void NavigateToTrack(TrackViewModel? viewModel)
        {
            if (viewModel != null)
                WeakReferenceMessenger.Default.Send(new TrackViewNavigationRequested(viewModel));
        }
    }
}
