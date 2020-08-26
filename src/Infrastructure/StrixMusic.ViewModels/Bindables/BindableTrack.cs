using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using StrixMusic.CoreInterfaces;
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

        /// <summary>
        /// Creates a bindable wrapper around an <see cref="ITrack"/>.
        /// </summary>
        /// <param name="track">The <see cref="ITrack"/> to wrap.</param>
        public BindableTrack(ITrack track)
        {
            _track = track;
            PlayAsyncCommand = new AsyncRelayCommand(PlayAsync);
            PauseAsyncCommand = new AsyncRelayCommand(PlayAsync);
        }

        /// <inheritdoc cref="IPlayable.Url"/>
        public Uri? Url => _track.Url;

        /// <inheritdoc cref="ITrack.Type"/>
        public string Type => _track.Type;

        /// <inheritdoc cref="ITrack.Artists"/>
        public ObservableCollection<IArtist> Artist => new ObservableCollection<IArtist>(_track.Artists);

        /// <inheritdoc cref="ITrack.Album"/>
        public IAlbum? Album => _track.Album;

        /// <inheritdoc cref="ITrack.DatePublished"/>
        public DateTime? DatePublished => _track.DatePublished;

        /// <inheritdoc cref="ITrack.Genres"/>
        public ObservableCollection<string>? Genres => new ObservableCollection<string>(_track.Genres);

        /// <inheritdoc cref="ITrack.TrackNumber"/>
        public int? TrackNumber => _track.TrackNumber;

        /// <inheritdoc cref="ITrack.PlayCount"/>
        public int? PlayCount => _track.PlayCount;

        /// <inheritdoc cref="ITrack.Language"/>
        public string? Language => _track.Language;

        /// <inheritdoc cref="ITrack.Lyrics"/>
        public ILyrics? Lyrics => _track.Lyrics;

        /// <inheritdoc cref="ITrack.IsExplicit"/>
        public bool IsExplicit => _track.IsExplicit;

        /// <inheritdoc cref="IPlayable.Duration"/>
        public TimeSpan Duration => _track.Duration;

        /// <inheritdoc cref="IPlayable.SourceCore"/>
        public ICore SourceCore => _track.SourceCore;

        /// <inheritdoc/>
        public string Id => _track.Id;

        /// <inheritdoc/>
        public string Name => _track.Name;

        /// <inheritdoc/>
        public ObservableCollection<IImage> Images => new ObservableCollection<IImage>(_track.Images);

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

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<IArtist>> ArtistsChanged
        {
            add
            {
                _track.ArtistsChanged += value;
            }

            remove
            {
                _track.ArtistsChanged -= value;
            }
        }

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<string>> GenresChanged
        {
            add
            {
                _track.GenresChanged += value;
            }

            remove
            {
                _track.GenresChanged -= value;
            }
        }

        /// <inheritdoc/>
        public event EventHandler<IAlbum?> AlbumChanged
        {
            add
            {
                _track.AlbumChanged += value;
            }

            remove
            {
                _track.AlbumChanged -= value;
            }
        }

        /// <inheritdoc/>
        public event EventHandler<DateTime?> DatePublishedChanged
        {
            add
            {
                _track.DatePublishedChanged += value;
            }

            remove
            {
                _track.DatePublishedChanged -= value;
            }
        }

        /// <inheritdoc/>
        public event EventHandler<int?> TrackNumberChanged
        {
            add
            {
                _track.TrackNumberChanged += value;
            }

            remove
            {
                _track.TrackNumberChanged -= value;
            }
        }

        /// <inheritdoc/>
        public event EventHandler<int?> PlayCountChanged
        {
            add
            {
                _track.PlayCountChanged += value;
            }

            remove
            {
                _track.PlayCountChanged -= value;
            }
        }

        /// <inheritdoc/>
        public event EventHandler<string?> LanguageChanged
        {
            add
            {
                _track.LanguageChanged += value;
            }

            remove
            {
                _track.LanguageChanged -= value;
            }
        }

        /// <inheritdoc/>
        public event EventHandler<ILyrics?> LyricsChanged
        {
            add
            {
                _track.LyricsChanged += value;
            }

            remove
            {
                _track.LyricsChanged -= value;
            }
        }

        /// <inheritdoc/>
        public event EventHandler<bool> IsExplicitChanged
        {
            add
            {
                _track.IsExplicitChanged += value;
            }

            remove
            {
                _track.IsExplicitChanged -= value;
            }
        }

        /// <inheritdoc/>
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
        public IReadOnlyList<IArtist> Artists => _track.Artists;


        /// <inheritdoc/>
        public Task PlayAsync()
        {
            return _track.PlayAsync();
        }
    }
}
