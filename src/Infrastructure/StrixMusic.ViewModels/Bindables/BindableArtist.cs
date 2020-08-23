using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using StrixMusic.CoreInterfaces;
using StrixMusic.CoreInterfaces.Enums;
using StrixMusic.CoreInterfaces.Interfaces;

namespace StrixMusic.ViewModels.Bindables
{
    /// <summary>
    /// Contains bindable information about an <see cref="IArtist"/>
    /// </summary>
    public class BindableArtist : ObservableObject, IArtist
    {
        private IArtist _artist;

        /// <summary>
        /// Initializes a new instance of the <see cref="BindableArtist"/> class.
        /// </summary>
        /// <param name="artist"></param>
        public BindableArtist(IArtist artist)
        {
            _artist = artist;
        }

        /// <inheritdoc/>
        public IReadOnlyList<IAlbum> Albums => _artist.Albums;

        /// <inheritdoc/>
        public int TotalAlbumsCount => _artist.TotalAlbumsCount;

        /// <inheritdoc/>
        public IReadOnlyList<IArtist> Artists => _artist.Artists;

        /// <inheritdoc/>
        public int TotalArtistsCount => _artist.TotalArtistsCount;

        /// <inheritdoc/>
        public IReadOnlyList<ITrack> Tracks => _artist.Tracks;

        /// <inheritdoc/>
        public int TotalTracksCount => _artist.TotalTracksCount;

        /// <inheritdoc/>
        public Uri? Url => _artist.Url;

        /// <inheritdoc/>
        public IUserProfile? Owner => _artist.Owner;

        /// <inheritdoc/>
        public ICore SourceCore => _artist.SourceCore;

        /// <inheritdoc/>
        public string Id => _artist.Id;

        /// <inheritdoc/>
        public string Name => _artist.Name;

        /// <inheritdoc/>
        public IReadOnlyList<IImage> Images => _artist.Images;

        /// <inheritdoc/>
        public string? Description => _artist.Description;

        /// <inheritdoc/>
        public PlaybackState PlaybackState => _artist.PlaybackState;

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<IAlbum>>? AlbumsChanged
        {
            add
            {
                _artist.AlbumsChanged += value;
            }

            remove
            {
                _artist.AlbumsChanged -= value;
            }
        }

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<IArtist>>? ArtistsChanged
        {
            add
            {
                _artist.ArtistsChanged += value;
            }

            remove
            {
                _artist.ArtistsChanged -= value;
            }
        }

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<ITrack>>? TracksChanged
        {
            add
            {
                _artist.TracksChanged += value;
            }

            remove
            {
                _artist.TracksChanged -= value;
            }
        }

        /// <inheritdoc/>
        public event EventHandler<PlaybackState>? PlaybackStateChanged
        {
            add
            {
                _artist.PlaybackStateChanged += value;
            }

            remove
            {
                _artist.PlaybackStateChanged -= value;
            }
        }

        /// <inheritdoc/>
        public Task PauseAsync()
        {
            return _artist.PauseAsync();
        }

        /// <inheritdoc/>
        public Task PlayAsync()
        {
            return _artist.PlayAsync();
        }

        /// <inheritdoc/>
        public Task PopulateAlbumsAsync(int limit, int offset = 0)
        {
            return _artist.PopulateAlbumsAsync(limit, offset);
        }

        /// <inheritdoc/>
        public Task PopulateArtistsAsync(int limit, int offset = 0)
        {
            return _artist.PopulateArtistsAsync(limit, offset);
        }

        /// <inheritdoc/>
        public Task PopulateTracksAsync(int limit, int offset = 0)
        {
            return _artist.PopulateTracksAsync(limit, offset);
        }
    }
}
