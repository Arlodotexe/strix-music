using Microsoft.Toolkit.Mvvm.ComponentModel;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.Groove.ViewModels.Pages.Interfaces;
using System;
using Windows.UI;

namespace StrixMusic.Shells.Groove.ViewModels.Pages
{
    public class GroovePlaylistPageViewModel : ObservableObject, IGroovePageViewModel
    {
        private PlaylistViewModel? _playlistViewModel;
        private Color? _backgroundColor;

        public GroovePlaylistPageViewModel(PlaylistViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        public event EventHandler? ViewModelSet;

        public string PageTitleResource => "MyMusic";

        public PlaylistViewModel? ViewModel
        {
            get => _playlistViewModel;
            set
            {
                SetProperty(ref _playlistViewModel, value);
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
