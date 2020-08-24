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
        /// <inheritdoc/>
        /// </summary>
        private ISearchResults _searchResults;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public BindableSearchResults(ISearchResults searchResults)
        {
            _searchResults = searchResults;
        }

        /// <summary>
        /// The Track search results across all cores.
        /// </summary>
        public ITrackCollection Tracks => _searchResults.Tracks;

        /// <summary>
        /// The Artists search results across all cores.
        /// </summary>
        public IArtistCollection Artists => _searchResults.Artists;

        /// <summary>
        /// The Albums search results across all cores.
        /// </summary>
        public IAlbumCollection Albums => _searchResults.Albums;

        /// <summary>
        /// The Playlists search results across all cores.
        /// </summary>
        public IPlaylistCollection Playlists => _searchResults.Playlists;
    }
}
