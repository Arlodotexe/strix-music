using Microsoft.Toolkit.Mvvm.ComponentModel;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.Groove.ViewModels.Pages.Interfaces;
using System;
using Windows.UI;

namespace StrixMusic.Shells.Groove.ViewModels.Pages
{
    public class GrooveHomePageViewModel : ObservableObject, IGroovePageViewModel
    {
        private LibraryViewModel? _libraryViewModel;
        private Color? _backgroundColor;

        public GrooveHomePageViewModel(LibraryViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        public event EventHandler? ViewModelSet;

        public string PageTitleResource => "MyMusic";

        public LibraryViewModel? ViewModel
        {
            get => _libraryViewModel;
            set
            {
                SetProperty(ref _libraryViewModel, value);
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
