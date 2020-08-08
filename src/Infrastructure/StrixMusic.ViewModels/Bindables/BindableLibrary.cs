using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using StrixMusic.CoreInterfaces.Enums;
using StrixMusic.CoreInterfaces.Interfaces;

namespace StrixMusic.ViewModels.Bindables
{
    /// <inheritdoc/>
    public class BindableLibrary : ObservableObject, ILibrary
    {
        private readonly ILibrary _library;

        /// <summary>
        /// Initializes a new instance of the <see cref="BindableLibrary"/> class.
        /// </summary>
        /// <param name="library"></param>
        public BindableLibrary(ILibrary library)
        {
            _library = library;
        }

        /// <inheritdoc/>
        public IReadOnlyList<IArtist> Artists => _library.Artists;

        /// <inheritdoc/>
        public int TotalArtistsCount => _library.TotalArtistsCount;

        /// <inheritdoc/>
        public IReadOnlyList<IAlbum> Albums => _library.Albums;

        /// <inheritdoc/>
        public int TotalAlbumsCount => _library.TotalAlbumsCount;

        /// <inheritdoc/>
        public IReadOnlyList<ITrack> Tracks => _library.Tracks;

        /// <inheritdoc/>
        public int TotalTracksCount => _library.TotalTracksCount;

        /// <inheritdoc/>
        public IReadOnlyList<IPlaylist> Playlists => _library.Playlists;

        /// <inheritdoc/>
        public string Id => _library.Id;

        /// <inheritdoc/>
        public ICore SourceCore => _library.SourceCore;

        /// <inheritdoc/>
        public string Name => _library.Name;

        /// <inheritdoc/>
        public IReadOnlyList<IImage> Images => _library.Images;

        /// <inheritdoc/>
        public Uri Url => _library.Url;

        /// <inheritdoc/>
        public string? Description => _library.Description;

        /// <inheritdoc/>
        public IUserProfile? Owner => _library.Owner;

        /// <inheritdoc/>
        public PlaybackState State => _library.State;

        /// <inheritdoc/>
        public ITrack? PlayingTrack => _library.PlayingTrack;

        /// <inheritdoc/>
        public int TotalPlaylistCount => _library.TotalPlaylistCount;

        /// <inheritdoc/>
        public void Pause()
        {
            _library.Pause();
        }

        /// <inheritdoc/>
        public void Play()
        {
            _library.Play();
        }

        /// <inheritdoc/>
        public Task PopulateAlbums(int limit, int offset = 0) => _library.PopulateAlbums(limit, offset);

        /// <inheritdoc/>
        public Task PopulateArtists(int limit, int offset = 0) => _library.PopulateArtists(limit, offset);

        /// <inheritdoc/>
        public Task PopulateTracks(int limit, int offset = 0) => _library.PopulateTracks(limit, offset);

        /// <inheritdoc/>
        public Task PopulatePlaylists(int limit, int offset) => _library.PopulatePlaylists(limit, offset);
    }
}
