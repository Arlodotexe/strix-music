using System.Collections.ObjectModel;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using StrixMusic.CoreInterfaces.Interfaces;

namespace StrixMusic.ViewModels.Bindables
{
    /// <summary>
    /// Used to bind search results across multiple cores to the View model.
    /// </summary>
    public class BindableSearchResults : ObservableObject
    {
        /// <summary>
        /// The Track search results across all cores.
        /// </summary>
        public ObservableCollection<ITrackCollection> Tracks { get; } = new ObservableCollection<ITrackCollection>();

        /// <summary>
        /// The Artists search results across all cores.
        /// </summary>
        public ObservableCollection<IArtistCollection> Artists { get; } = new ObservableCollection<IArtistCollection>();

        /// <summary>
        /// The Albums search results across all cores.
        /// </summary>
        public ObservableCollection<IAlbumCollection> Albums { get; } = new ObservableCollection<IAlbumCollection>();

        /// <summary>
        /// The Playlists search results across all cores.
        /// </summary>
        public ObservableCollection<IPlaylistCollection> Playlists { get; } = new ObservableCollection<IPlaylistCollection>();
    }
}
