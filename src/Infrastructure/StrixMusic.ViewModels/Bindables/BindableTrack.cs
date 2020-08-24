using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using StrixMusic.CoreInterfaces.Enums;
using StrixMusic.CoreInterfaces.Interfaces;

namespace StrixMusic.ViewModels.Bindables
{
    /// <summary>
    /// Contains bindable information about an <see cref="ITrack"/>
    /// </summary>
    public class BindableTrack : ObservableObject
    {
        private ITrack _track;

        /// <inheritdoc/>
        public BindableTrack(ITrack track)
        {
            _track = track;
            PlayAsyncCommand = new AsyncRelayCommand(PlayAsync);
            PauseAsyncCommand = new AsyncRelayCommand(PlayAsync);
        }

        /// <inheritdoc/>
        public Uri? Url => _track.Url;

        /// <inheritdoc/>
        public string Type => _track.Type;

        /// <inheritdoc/>
        public ObservableCollection<IArtist> Artists => (ObservableCollection<IArtist>)_track.Artists;

        /// <inheritdoc/>
        public IAlbum? Album => _track.Album;

        /// <inheritdoc/>
        public DateTime? DatePublished => _track.DatePublished;

        /// <inheritdoc/>
        public ObservableCollection<string>? Genres => (ObservableCollection<string>?)_track.Genres;

        /// <inheritdoc/>
        public int? TrackNumber => _track.TrackNumber;

        /// <inheritdoc/>
        public int? PlayCount => _track.PlayCount;

        /// <inheritdoc/>
        public string? Language => _track.Language;

        /// <inheritdoc/>
        public ILyrics? Lyrics => _track.Lyrics;

        /// <inheritdoc/>
        public bool IsExplicit => _track.IsExplicit;

        /// <inheritdoc/>
        public TimeSpan Duration => _track.Duration;

        /// <inheritdoc/>
        public ICore SourceCore => _track.SourceCore;

        /// <inheritdoc/>
        public string Id => _track.Id;

        /// <inheritdoc/>
        public string Name => _track.Name;

        /// <inheritdoc/>
        public ObservableCollection<IImage> Images => (ObservableCollection<IImage>)_track.Images;

        /// <inheritdoc/>
        public string? Description => _track.Description;

        /// <inheritdoc/>
        public PlaybackState PlaybackState => _track.PlaybackState;

        /// <inheritdoc/>
        public event EventHandler<PlaybackState>? PlaybackStateChanged
        {
            add
            {
                _track.PlaybackStateChanged += value;
            }

            remove
            {
                _track.PlaybackStateChanged -= value;
            }
        }

        /// <summary>
        /// Attempts to play the track, if paused.
        /// </summary>
        public IAsyncRelayCommand PlayAsyncCommand { get; }

        /// <inheritdoc/>
        public Task PauseAsync()
        {
            return _track.PauseAsync();
        }

        /// <summary>
        /// Attempts to pause the track, if playing.
        /// </summary>
        public IAsyncRelayCommand PauseAsyncCommand { get; }

        /// <inheritdoc/>
        public Task PlayAsync()
        {
            return _track.PlayAsync();
        }
    }
}
