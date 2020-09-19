using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hqub.MusicBrainz.API;
using Hqub.MusicBrainz.API.Entities;
using Microsoft.Extensions.DependencyInjection;
using StrixMusic.Core.MusicBrainz.Statics;
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
        public Uri Url => new Uri($"https://musicbrainz.org/artist/{Id}");

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
            var releasesList = await _musicBrainzClient.Releases.BrowseAsync("artist", Id, limit, offset);

            foreach (var release in releasesList.Items)
            {
                foreach (var medium in release.Media)
                {
                    _albums.Add(new MusicBrainzAlbum(SourceCore, release, medium));
                }
            }

            return _albums;
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<ITrack>> PopulateTracksAsync(int limit, int offset)
        {
            var recordings = await _musicBrainzClient.Recordings.BrowseAsync("artist", Id, limit, offset, RelationshipQueries.Recordings);

            var releases =
                await _musicBrainzClient.Releases.BrowseAsync("artist", Id, 100, 0, RelationshipQueries.Releases);

            var releasesList = releases.Items.ToList();

            for (var i = 100; i < releasesList.Count; i += 100)
            {
                var nextReleasesPage = await _musicBrainzClient.Releases.BrowseAsync("artist", Id, 100, 0, RelationshipQueries.Releases);

                releasesList.AddRange(nextReleasesPage.Items);
            }

            foreach (var recording in recordings.Items)
            {
                foreach (var release in releasesList)
                {
                    // Iterate through each physical medium for this release.
                    foreach (var medium in release.Media)
                    {
                        // Iterate the tracks and find a matching ID for this recording
                        foreach (var track in medium.Tracks.Where(track => track.Recording.Id == recording.Id))
                        {
                            _tracks.Add(new MusicBrainzTrack(SourceCore, track, new MusicBrainzAlbum(SourceCore, release, medium)));
                        }
                    }
                }
            }

            return _tracks;
        }
    }
}
