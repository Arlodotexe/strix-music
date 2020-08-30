using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.Input;
using StrixMusic.CoreInterfaces;
using StrixMusic.CoreInterfaces.Enums;
using StrixMusic.CoreInterfaces.Interfaces;

namespace StrixMusic.ViewModels.Bindables
{
    /// <summary>
    /// A bindable wrapper for <see cref="IPlaylist"/>.
    /// </summary>
    public class BindablePlaylist : BindableMergeableObject<IPlaylist>
    {
        private readonly IPlaylist _playlist;

        /// <summary>
        /// Initializes a new instance of the <see cref="BindablePlaylist"/> class.
        /// </summary>
        /// <param name="playlist">The <see cref="IPlaylist"/> to wrap.</param>
        public BindablePlaylist(IPlaylist playlist)
        {
            _playlist = playlist;

            PauseAsyncCommand = new AsyncRelayCommand(PauseAsync);
            PlayAsyncCommand = new AsyncRelayCommand(PlayAsync);

            if (_playlist.Owner != null)
                _owner = new BindableUserProfile(_playlist.Owner);
            Tracks = new ObservableCollection<BindableTrack>(_playlist.Tracks.Select(x => new BindableTrack(x)));
            Images = new ObservableCollection<IImage>(_playlist.Images);
            RelatedItems = new BindableCollectionGroup(_playlist.RelatedItems);
            SourceCore = new BindableCore(_playlist.SourceCore);

            AttachEvents();
        }

        private void AttachEvents()
        {
            _playlist.DescriptionChanged += Playlist_DescriptionChanged;
            _playlist.ImagesChanged += Playlist_ImagesChanged;
            _playlist.NameChanged += Playlist_NameChanged;
            _playlist.OwnerChanged += Playlist_OwnerChanged;
            _playlist.PlaybackStateChanged += Playlist_PlaybackStateChanged;
            _playlist.TracksChanged += Playlist_TracksChanged;
            _playlist.UrlChanged += Playlist_UrlChanged;
        }

        private void DetachEvents()
        {
            _playlist.DescriptionChanged -= Playlist_DescriptionChanged;
            _playlist.ImagesChanged -= Playlist_ImagesChanged;
            _playlist.NameChanged -= Playlist_NameChanged;
            _playlist.OwnerChanged -= Playlist_OwnerChanged;
            _playlist.PlaybackStateChanged -= Playlist_PlaybackStateChanged;
            _playlist.TracksChanged -= Playlist_TracksChanged;
            _playlist.UrlChanged -= Playlist_UrlChanged;
        }

        private void Playlist_UrlChanged(object sender, Uri? e)
        {
            Url = e;
        }

        private void Playlist_TracksChanged(object sender, CollectionChangedEventArgs<ITrack> e)
        {
            foreach (var item in e.AddedItems)
            {
                Tracks.Add(new BindableTrack(item));
            }

            foreach (var item in e.RemovedItems)
            {
                Tracks.Remove(new BindableTrack(item));
            }
        }

        private void Playlist_OwnerChanged(object sender, IUserProfile e)
        {
            Owner = new BindableUserProfile(e);
        }

        private void Playlist_PlaybackStateChanged(object sender, PlaybackState e)
        {
            PlaybackState = e;
        }

        private void Playlist_NameChanged(object sender, string e)
        {
            Name = e;
        }

        private void Playlist_ImagesChanged(object sender, CollectionChangedEventArgs<IImage> e)
        {
            foreach (var item in e.AddedItems)
            {
                Images.Add(item);
            }

            foreach (var item in e.RemovedItems)
            {
                Images.Remove(item);
            }
        }

        private void Playlist_DescriptionChanged(object sender, string? e)
        {
            Description = e;
        }

        private BindableUserProfile? _owner;

        /// <inheritdoc cref="IPlaylist.Owner"/>
        public BindableUserProfile? Owner 
        {
            get => _owner;
            set => SetProperty(ref _owner, value);
        }

        /// <inheritdoc cref="ITrackCollection.Tracks"/>
        public ObservableCollection<BindableTrack> Tracks { get; }

        /// <inheritdoc cref="ITrackCollection.TotalTracksCount"/>
        public int TotalTracksCount => _playlist.TotalTracksCount;

        /// <inheritdoc cref="IPlayable.SourceCore"/>
        public BindableCore SourceCore { get; }

        /// <inheritdoc cref="IPlayable.Id"/>
        public string Id => _playlist.Id;

        /// <inheritdoc cref="IPlayable.Url"/>
        public Uri? Url
        {
            get => _playlist.Url;
            set => SetProperty(() => _playlist.Url, value);
        }

        /// <inheritdoc cref="IPlayable.Name"/>
        public string Name
        {
            get => _playlist.Name;
            set => SetProperty(() => _playlist.Name, value);
        }

        /// <inheritdoc cref="IPlayable.Images"/>
        public ObservableCollection<IImage> Images { get; }

        /// <inheritdoc cref="IPlayable.Description"/>
        public string? Description
        {
            get => _playlist.Description;
            set => SetProperty(() => _playlist.Description, value);
        }

        /// <inheritdoc cref="IPlayable.PlaybackState"/>
        public PlaybackState PlaybackState
        {
            get => _playlist.PlaybackState;
            set => SetProperty(() => _playlist.PlaybackState, value);
        }

        /// <inheritdoc cref="IPlayable.Duration"/>
        public TimeSpan Duration => _playlist.Duration;

        /// <inheritdoc cref="IRelatedCollectionGroups.RelatedItems"/>
        public BindableCollectionGroup RelatedItems { get; }

        /// <inheritdoc cref="IPlaylist.OwnerChanged"/>
        public event EventHandler<IUserProfile> OwnerChanged
        {
            add
            {
                _playlist.OwnerChanged += value;
            }

            remove
            {
                _playlist.OwnerChanged -= value;
            }
        }

        /// <inheritdoc cref="ITrackCollection.TracksChanged"/>
        public event EventHandler<CollectionChangedEventArgs<ITrack>>? TracksChanged
        {
            add
            {
                _playlist.TracksChanged += value;
            }

            remove
            {
                _playlist.TracksChanged -= value;
            }
        }

        /// <inheritdoc cref="IPlayable.PlaybackStateChanged"/>
        public event EventHandler<PlaybackState> PlaybackStateChanged
        {
            add
            {
                _playlist.PlaybackStateChanged += value;
            }

            remove
            {
                _playlist.PlaybackStateChanged -= value;
            }
        }

        /// <inheritdoc cref="IPlayable.NameChanged"/>
        public event EventHandler<string> NameChanged
        {
            add
            {
                _playlist.NameChanged += value;
            }

            remove
            {
                _playlist.NameChanged -= value;
            }
        }

        /// <inheritdoc cref="IPlayable.DescriptionChanged"/>
        public event EventHandler<string?> DescriptionChanged
        {
            add
            {
                _playlist.DescriptionChanged += value;
            }

            remove
            {
                _playlist.DescriptionChanged -= value;
            }
        }

        /// <inheritdoc cref="IPlayable.UrlChanged"/>
        public event EventHandler<Uri?> UrlChanged
        {
            add
            {
                _playlist.UrlChanged += value;
            }

            remove
            {
                _playlist.UrlChanged -= value;
            }
        }

        /// <inheritdoc cref="IPlayable.ImagesChanged"/>
        public event EventHandler<CollectionChangedEventArgs<IImage>>? ImagesChanged
        {
            add
            {
                _playlist.ImagesChanged += value;
            }

            remove
            {
                _playlist.ImagesChanged -= value;
            }
        }

        /// <summary>
        /// Attempts to pause the playlist.
        /// </summary>
        public IAsyncRelayCommand PauseAsyncCommand { get; }

        /// <inheritdoc cref="IPlayable.PauseAsync"/>
        public Task PauseAsync()
        {
            return _playlist.PauseAsync();
        }

        /// <summary>
        /// Attempts to play the playlist.
        /// </summary>
        public IAsyncRelayCommand PlayAsyncCommand { get; }

        /// <inheritdoc cref="IPlayable.PlayAsync"/>
        public Task PlayAsync()
        {
            return _playlist.PlayAsync();
        }

        /// <inheritdoc cref="ITrackCollection.PopulateTracksAsync"/>
        public Task<IReadOnlyList<ITrack>> PopulateTracksAsync(int limit, int offset = 0)
        {
            return _playlist.PopulateTracksAsync(limit, offset);
        }
    }
}
