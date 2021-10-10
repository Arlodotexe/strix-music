using Microsoft.Toolkit.Mvvm.ComponentModel;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.Groove.ViewModels.Pages.Interfaces;
using System;
using Windows.UI;

namespace StrixMusic.Shells.Groove.ViewModels.Pages
{
    public class GrooveArtistPageViewModel : ObservableObject, IGroovePageViewModel
    {
        private ArtistViewModel? _artistViewModel;
        private Color? _backgroundColor;

        public GrooveArtistPageViewModel(ArtistViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        public event EventHandler? ViewModelSet;

        public string PageTitleResource => "Artist";

        public ArtistViewModel? ViewModel
        {
            get => _artistViewModel;
            set
            {
                SetProperty(ref _artistViewModel, value);
                ViewModelSet?.Invoke(this, EventArgs.Empty);
            }
        }

        public Color? BackgroundColor
        {
            get => _backgroundColor;
            set => SetProperty(ref _backgroundColor, value);
        }
    }
}
