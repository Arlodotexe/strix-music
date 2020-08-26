using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using StrixMusic.CoreInterfaces.Enums;
using StrixMusic.CoreInterfaces.Interfaces;

namespace StrixMusic.ViewModels.Bindables
{
    /// <inheritdoc/>
    public class BindableAlbum : ObservableObject, IAlbum
    {
        private readonly IAlbum _album;

        /// <summary>
        /// Initializes a new instance of the <see cref="BindableAlbum"/> class.
        /// </summary>
        /// <param name="album"><inheritdoc cref="IAlbum"/></param>
        public BindableAlbum(IAlbum album)
        {
            _album = album;

            PauseAsyncCommand = new AsyncRelayCommand(PauseAsync);
            PlayAsyncCommand = new AsyncRelayCommand(PlayAsync);

            AttachEvents();
        }

        private void AttachEvents()
        {
            _album.PlaybackStateChanged += Album_PlaybackStateChanged;
            _album.DescriptionChanged += Album_DescriptionChanged;
            _album.NameChanged += Album_NameChanged;
            _album.UrlChanged += Album_UrlChanged;
        }

        private void DetachEvents()
        {
            _album.PlaybackStateChanged -= Album_PlaybackStateChanged;
            _album.DescriptionChanged -= Album_DescriptionChanged;
            _album.NameChanged -= Album_NameChanged;
            _album.UrlChanged -= Album_UrlChanged;
        }

        private void Album_UrlChanged(object sender, Uri? e)
        {
            Url = e;
        }

        private void Album_NameChanged(object sender, string e)
        {
            Name = e;
        }

        private void Album_DescriptionChanged(object sender, string? e)
        {
            Description = e;
        }

        private void Album_PlaybackStateChanged(object sender, PlaybackState e)
        {
            PlaybackState = e;
        }

        /// <inheritdoc/>
        public IReadOnlyList<ITrack> Tracks
        {
            get => _album.Tracks;
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
        public string Name
        {
            get => _album.Name;
            set => SetProperty(() => _album.Name, value);
        }

        /// <inheritdoc/>
        public IReadOnlyList<IImage> Images => _album.Images;

        /// <inheritdoc/>
        public Uri? Url
        {
            get => _album.Url;
            set => SetProperty(() => _album.Url, value);
        }

        /// <inheritdoc/>
        public string? Description
        {
            get => _album.Description;
            set => SetProperty(() => _album.Description, value);
        }

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
        public event EventHandler<string>? NameChanged
        {
            add
            {
                _album.NameChanged += value;
            }

            remove
            {
                _album.NameChanged -= value;
            }
        }

        /// <inheritdoc/>
        public event EventHandler<string?> DescriptionChanged
        {
            add
            {
                _album.DescriptionChanged += value;
            }

            remove
            {
                _album.DescriptionChanged -= value;
            }
        }

        /// <inheritdoc/>
        public event EventHandler<Uri?> UrlChanged
        {
            add
            {
                _album.UrlChanged += value;
            }

            remove
            {
                _album.UrlChanged -= value;
            }
        }

        /// <summary>
        /// Attempts to pause the album, if playing.
        /// </summary>
        public IAsyncRelayCommand PauseAsyncCommand { get; }

        /// <inheritdoc/>
        public Task PauseAsync() => _album.PauseAsync();

        /// <summary>
        /// Attempts to play the album.
        /// </summary>
        public IAsyncRelayCommand PlayAsyncCommand { get; }

        /// <inheritdoc/>
        public TimeSpan Duration => _album.Duration;

        /// <inheritdoc/>
        public Task PlayAsync() => _album.PlayAsync();

        /// <inheritdoc/>
        public Task PopulateTracksAsync(int limit, int offset = 0) => _album.PopulateTracksAsync(limit, offset);
    }
}
