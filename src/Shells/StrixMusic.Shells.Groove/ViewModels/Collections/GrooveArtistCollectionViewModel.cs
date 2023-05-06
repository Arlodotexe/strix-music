﻿using CommunityToolkit.Mvvm.ComponentModel;
using StrixMusic.Sdk.ViewModels;

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
        public GrooveArtistCollectionViewModel()
        {
        }

        /// <summary>
        /// The <see cref="IArtistCollectionViewModel"/> inside this ViewModel on display.
        /// </summary>
        public IArtistCollectionViewModel? ArtistCollection
        {
            get => _artistCollectionViewModel;
            set => SetProperty(ref _artistCollectionViewModel, value);
        }
    }
}
