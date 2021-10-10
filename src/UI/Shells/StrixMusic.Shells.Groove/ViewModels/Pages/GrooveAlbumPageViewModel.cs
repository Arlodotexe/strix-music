﻿using Microsoft.Toolkit.Mvvm.ComponentModel;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.Groove.ViewModels.Pages.Interfaces;
using System;
using Windows.UI;

namespace StrixMusic.Shells.Groove.ViewModels.Pages
{
    public class GrooveAlbumPageViewModel : ObservableObject, IGroovePageViewModel
    {
        private AlbumViewModel? _albumViewModel;
        private Color? _backgroundColor;

        public GrooveAlbumPageViewModel(AlbumViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        public event EventHandler? ViewModelSet;

        public string PageTitleResource => "Album";

        public AlbumViewModel? ViewModel
        {
            get => _albumViewModel;
            set
            {
                SetProperty(ref _albumViewModel, value);
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
