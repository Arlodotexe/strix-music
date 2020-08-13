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
        /// <summary>
        /// The Track recently played across all cores.
        /// </summary>
        public ObservableCollection<ITrackCollection> Tracks { get; } = new ObservableCollection<ITrackCollection>();

        /// <summary>
        /// The Artists recently played across all cores.
        /// </summary>
        public ObservableCollection<IArtistCollection> Artists { get; } = new ObservableCollection<IArtistCollection>();

        /// <summary>
        /// The Albums recently played across all cores.
        /// </summary>
        public ObservableCollection<IAlbumCollection> Albums { get; } = new ObservableCollection<IAlbumCollection>();

        /// <summary>
        /// The Playlists recently played across all cores.
        /// </summary>
        public ObservableCollection<IPlaylistCollection> Playlists { get; } = new ObservableCollection<IPlaylistCollection>();
    }
}
