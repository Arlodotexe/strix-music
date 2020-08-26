using System.Collections.ObjectModel;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using StrixMusic.CoreInterfaces.Interfaces;

namespace StrixMusic.ViewModels.Bindables
{
    /// <summary>
    /// Used to bind recently played across multiple cores to the View model.
    /// </summary>
    public class BindableRecentlyPlayed : ObservableObject
    {
        /// <inheritdoc cref="ISearchResults.Tracks"/>
        public ObservableCollection<ITrackCollection> Tracks { get; } = new ObservableCollection<ITrackCollection>();

        /// <inheritdoc cref="ISearchResults.Artists"/>
        public ObservableCollection<IArtistCollection> Artists { get; } = new ObservableCollection<IArtistCollection>();

        /// <inheritdoc cref="ISearchResults.Albums"/>
        public ObservableCollection<IAlbumCollection> Albums { get; } = new ObservableCollection<IAlbumCollection>();

        /// <inheritdoc cref="ISearchResults.Playlists"/>
        public ObservableCollection<IPlaylistCollection> Playlists { get; } = new ObservableCollection<IPlaylistCollection>();
    }
}
