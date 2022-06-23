using System;
using System.Collections.Generic;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using StrixMusic.Sdk.ViewModels;

namespace StrixMusic.Shells.ZuneDesktop.Controls.Views.Collection
{
    /// <summary>
    /// An container for items in a <see cref="ZuneAlbumCollectionItem"/>, with added functionality and observable properties.
    /// </summary>
    public partial class ZuneAlbumCollectionItem : ObservableObject
    {
        [ObservableProperty]
        private IAlbumCollectionViewModel? _parentCollection;

        [ObservableProperty]
        private AlbumViewModel? _album;

        /// <summary>
        /// Emits the <see cref="ZuneAlbumCollectionItem"/> whose collection needs to be played.
        /// </summary>
        public event EventHandler<AlbumViewModel>? AlbumPlaybackTriggered;
    }
}
