using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using StrixMusic.CoreInterfaces;
using StrixMusic.CoreInterfaces.Enums;
using StrixMusic.CoreInterfaces.Interfaces;

namespace StrixMusic.ViewModels.Bindables
{
    /// <summary>
    /// Contains bindable information about an <see cref="IArtist"/>
    /// </summary>
    public class BindableArtist : ObservableObject
    {
        private IArtist _artist;

        /// <summary>
        /// Initializes a new instance of the <see cref="BindableArtist"/> class.
        /// </summary>
        /// <param name="artist"></param>
        public BindableArtist(IArtist artist)
        {
            _artist = artist;

            PlayAsyncCommand = new AsyncRelayCommand(PlayAsync);
            PauseAsyncCommand = new AsyncRelayCommand(PauseAsync);

            Tracks = new ObservableCollection<ITrack>(_artist.Tracks);
            Albums = new ObservableCollection<IAlbum>(_artist.Albums);
            Artists = new ObservableCollection<IArtist>(_artist.RelatedArtists);

            AttachEvents();
        }

        private void AttachEvents()
        {
            _artist.PlaybackStateChanged += Artist_PlaybackStateChanged;
            _artist.TracksChanged += Artist_RelatedTracksChanged;
            _artist.AlbumsChanged += Artist_AlbumsChanged;
            _artist.RelatedArtistsChanged += Artist_RelatedArtistsChanged;
        }

        private void DetachEvents()
        {
            _artist.PlaybackStateChanged -= Artist_PlaybackStateChanged;
            _artist.TracksChanged -= Artist_RelatedTracksChanged;
            _artist.AlbumsChanged -= Artist_AlbumsChanged;
            _artist.RelatedArtistsChanged -= Artist_RelatedArtistsChanged;
        }

        private void Artist_AlbumsChanged(object sender, CollectionChangedEventArgs<IAlbum> e)
        {
            foreach (var item in e.RemovedItems)
            {
                Albums.Remove(item);
            }

            foreach (var item in e.AddedItems)
            {
                Albums.Add(item);
            }
        }

        private void Artist_RelatedArtistsChanged(object sender, CollectionChangedEventArgs<IArtist> e)
        {
            foreach (var item in e.RemovedItems)
            {
                Artists.Remove(item);
            }

            foreach (var item in e.AddedItems)
            {
                Artists.Add(item);
            }
        }

        private void Artist_RelatedTracksChanged(object sender, CollectionChangedEventArgs<ITrack> e)
        {
            foreach (var item in e.RemovedItems)
            {
                Tracks.Remove(item);
            }

            foreach (var item in e.AddedItems)
            {
                Tracks.Add(item);
            }
        }

        private void Artist_PlaybackStateChanged(object sender, PlaybackState e)
        {
            PlaybackState = e;
        }

        /// <inheritdoc/>
        public ObservableCollection<IAlbum> Albums { get; }

        /// <inheritdoc/>
        public int TotalAlbumsCount => _artist.TotalAlbumsCount;

        /// <inheritdoc/>
        public ObservableCollection<IArtist> Artists { get; }

        /// <inheritdoc/>
        public int TotalArtistsCount => _artist.TotalRelatedArtistsCount;

        /// <inheritdoc/>
        public ObservableCollection<ITrack> Tracks { get; }

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
        public PlaybackState PlaybackState
        {
            get => _artist.PlaybackState;
            set => SetProperty(() => _artist.PlaybackState, value);
        }

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
                _artist.RelatedArtistsChanged += value;
            }

            remove
            {
                _artist.RelatedArtistsChanged -= value;
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

        /// <summary>
        /// Attempts to pause the artist, if playing.
        /// </summary>
        public IAsyncRelayCommand PauseAsyncCommand { get; }

        /// <inheritdoc/>
        public Task PauseAsync()
        {
            return _artist.PauseAsync();
        }

        /// <summary>
        /// Attempts to play the artist.
        /// </summary>
        public IAsyncRelayCommand PlayAsyncCommand { get; }

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
            return _artist.PopulateRelatedArtistsAsync(limit, offset);
        }

        /// <inheritdoc/>
        public Task PopulateTracksAsync(int limit, int offset = 0)
        {
            return _artist.PopulateTracksAsync(limit, offset);
        }
    }
}
