using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hqub.MusicBrainz.API;
using Hqub.MusicBrainz.API.Entities;
using Microsoft.Extensions.DependencyInjection;
using StrixMusic.Sdk;
using StrixMusic.Sdk.Enums;
using StrixMusic.Sdk.Events;
using StrixMusic.Sdk.Interfaces;

namespace StrixMusic.Core.MusicBrainz.Models
{
    /// <inheritdoc />
    public class MusicBrainzArtist : IArtist
    {
        private readonly Artist _artist;
        private readonly MusicBrainzClient _musicBrainzClient;
        private readonly List<IAlbum> _albums;
        private readonly List<ITrack> _tracks;

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicBrainzArtist"/> class.
        /// </summary>
        /// <param name="artist"></param>
        /// <param name="sourceCore"></param>
        public MusicBrainzArtist(ICore sourceCore, Artist artist)
        {
            SourceCore = sourceCore;
            _artist = artist;
            _albums = new List<IAlbum>();
            _tracks = new List<ITrack>();
            _musicBrainzClient = SourceCore.CoreConfig.Services.GetService<MusicBrainzClient>();
        }

        /// <inheritdoc/>
        public string Id => _artist.Id;

        /// <inheritdoc/>
        public IReadOnlyList<IAlbum> Albums => _albums;

        /// <inheritdoc/>
        public int TotalAlbumsCount => _artist.Releases.Count;

        /// <inheritdoc/>
        public IReadOnlyList<ITrack> Tracks => _tracks;

        /// <inheritdoc/>
        public int TotalTracksCount => throw new NotImplementedException();

        /// <inheritdoc/>
        public ICore SourceCore { get; }

        /// <inheritdoc/>
        public Uri Url => throw new NotImplementedException();

        /// <inheritdoc/>
        public string Name => _artist.Name;

        /// <inheritdoc/>
        public IReadOnlyList<IImage> Images => throw new NotImplementedException();

        /// <inheritdoc/>
        public string Description => _artist.SortName;

        /// <inheritdoc/>
        public PlaybackState PlaybackState => throw new NotImplementedException();

        /// <inheritdoc/>
        public TimeSpan Duration => throw new NotImplementedException();

        /// <inheritdoc/>
        public IPlayableCollectionGroup? RelatedItems => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool IsPlayAsyncSupported => false;

        /// <inheritdoc/>
        public bool IsPauseAsyncSupported => false;

        /// <inheritdoc/>
        public bool IsChangeNameAsyncSupported => false;

        /// <inheritdoc/>
        public bool IsChangeImagesAsyncSupported => false;

        /// <inheritdoc/>
        public bool IsChangeDescriptionAsyncSupported => false;

        /// <inheritdoc/>
        public bool IsChangeDurationAsyncSupported => false;

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<IAlbum>>? AlbumsChanged;

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<ITrack>>? TracksChanged;

        /// <inheritdoc/>
        public event EventHandler<PlaybackState>? PlaybackStateChanged;

        /// <inheritdoc/>
        public event EventHandler<string>? NameChanged;

        /// <inheritdoc/>
        public event EventHandler<string?>? DescriptionChanged;

        /// <inheritdoc/>
        public event EventHandler<Uri?>? UrlChanged;

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<IImage>>? ImagesChanged;

        /// <inheritdoc/>
        public event EventHandler<TimeSpan>? DurationChanged;

        /// <inheritdoc/>
        public Task ChangeDescriptionAsync(string? description)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public Task ChangeDurationAsync(TimeSpan duration)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public Task ChangeImagesAsync(IReadOnlyList<IImage> images)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public Task ChangeNameAsync(string name)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public Task PauseAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task PlayAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<IAlbum>> PopulateAlbumsAsync(int limit, int offset)
        {
            var releases = await _musicBrainzClient.Releases.SearchAsync("*", limit, offset);
            Parallel.ForEach(releases, (release) =>
            {
                Parallel.ForEach(release.Credits, (credit) =>
                {
                    if (credit.Artist.Id == Id)
                        _albums.Add(new MusicBrainzAlbum(SourceCore, release));
                });
            });

            return _albums;
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<ITrack>> PopulateTracksAsync(int limit, int offset)
        {
            var recordings = await _musicBrainzClient.Recordings.SearchAsync("*", limit, offset);
            Parallel.ForEach(recordings, (recording) =>
            {
                Parallel.ForEach(recording.Credits, (credit) =>
                {
                    if (credit.Artist.Id == Id)
                        _tracks.Add(new MusicBrainzTrack(SourceCore, recording));
                });
            });

            return _tracks;
        }
    }
}
