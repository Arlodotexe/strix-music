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
    public class BindableAlbum : ObservableObject
    {
        private readonly IAlbum _album;

        /// <summary>
        /// Initializes a new instance of the <see cref="BindableAlbum"/> class.
        /// </summary>
        /// <param name="album"><inheritdoc cref="IAlbum"/></param>
        public BindableAlbum(IAlbum album)
        {
            _album = album;
        }

        private void AttachEvents()
        {
            _album.PlaybackStateChanged += Album_PlaybackStateChanged;
        }

        private void DetachEvents()
        {
            _album.PlaybackStateChanged -= Album_PlaybackStateChanged;
        }

        private void Album_PlaybackStateChanged(object sender, PlaybackState e)
        {

        }

        /// <inheritdoc/>
        public List<ITrack> Tracks
        {
            get => (List<ITrack>)_album.Tracks;
            set => SetProperty(() => _album.Tracks, value);
        }

        /// <inheritdoc/>
        public int TotalTrackCount => _album.TotalTrackCount;

        /// <inheritdoc/>
        public IArtist Artist => _album.Artist;

        /// <inheritdoc/>
        public Uri? CoverImageUri => _album.CoverImageUri;

        /// <inheritdoc/>
        public string Id => _album.Id;

        /// <inheritdoc/>
        public ICore SourceCore => _album.SourceCore;

        /// <inheritdoc/>
        public string Name => _album.Name;

        /// <inheritdoc/>
        public IReadOnlyList<IImage> Images => _album.Images;

        /// <inheritdoc/>
        public Uri? Url => _album.Url;

        /// <inheritdoc/>
        public string? Description => _album.Description;

        /// <inheritdoc/>
        public IUserProfile? Owner => _album.Owner;

        /// <inheritdoc/>
        public PlaybackState PlaybackState
        {
            get => _album.PlaybackState;
            set => SetProperty(() => _album.PlaybackState, value);
        }

        /// <inheritdoc/>
        public event EventHandler<PlaybackState>? PlaybackStateChanged
        {
            add
            {
                _album.PlaybackStateChanged += value;
            }

            remove
            {
                _album.PlaybackStateChanged -= value;
            }
        }

        /// <inheritdoc/>
        public Task PauseAsync()
        {
            return _album.PauseAsync();
        }

        /// <inheritdoc/>
        public Task PlayAsync()
        {
            return _album.PlayAsync();
        }

        /// <inheritdoc/>
        public Task PopulateTracksAsync(int limit, int offset = 0)
        {
            return _album.PopulateTracksAsync(limit, offset);
        }
    }
}
