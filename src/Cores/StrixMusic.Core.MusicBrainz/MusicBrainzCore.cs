using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hqub.MusicBrainz.API;
using Microsoft.Extensions.DependencyInjection;
using OwlCore.Collections;
using StrixMusic.Core.MusicBrainz.Models;
using StrixMusic.Core.MusicBrainz.Services;
using StrixMusic.Core.MusicBrainz.Statics;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Core.MusicBrainz
{
    /// <summary>
    /// A core that gets data from the <see href="https://https://musicbrainz.org/doc/">MusicBrainz API</see>.
    /// </summary>
    /// <remarks>
    /// Backing API library: <see href="https://github.com/Arlodotexe/MusicBrainz/"/>
    /// </remarks>
    public class MusicBrainzCore : ICore
    {
        private MusicBrainzClient? _musicBrainzClient;
        private MusicBrainzArtistHelpersService? _artistHelperService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicBrainzCore"/> class.
        /// </summary>
        /// <param name="instanceId"></param>
        public MusicBrainzCore(string instanceId)
        {
            InstanceId = instanceId;
            CoreConfig = new MusicBrainzCoreConfig(this);

            // The library created here won't be used by the UI.
            // The UI isn't loaded until InitAsync is called, where we set up the actual library.
            Library = new MusicBrainzLibrary(this);
            Devices = new SynchronizedObservableCollection<ICoreDevice>();
            RecentlyPlayed = new MusicBrainzCoreRecentlyPlayed(this);
            Discoverables = new MusicBrainzCoreDiscoverables(this);
            User = new MusicBrainzCoreUser(this);
        }

        /// <inheritdoc/>
        public ICoreConfig CoreConfig { get; }

        /// <inheritdoc />
        public ICore SourceCore => this;

        /// <inheritdoc/>
        public CoreState CoreState { get; internal set; } = CoreState.Unloaded;

        /// <inheritdoc/>
        public string Name => "MusicBrainz";

        /// <inheritdoc/>
        public ICoreUser User { get; }

        /// <inheritdoc/>
        public SynchronizedObservableCollection<ICoreDevice> Devices { get; }

        /// <inheritdoc/>
        public ICoreLibrary Library { get; private set; }

        /// <inheritdoc/>
        public ICoreRecentlyPlayed RecentlyPlayed { get; }

        /// <inheritdoc/>
        public ICoreDiscoverables Discoverables { get; }

        /// <inheritdoc/>
        public string InstanceId { get; }

        /// <inheritdoc/>
        public ICorePlayableCollectionGroup? Pins { get; }

        /// <inheritdoc/>
        public Task<IMediaSourceConfig?> GetMediaSource(ICoreTrack track)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public event EventHandler<CoreState>? CoreStateChanged;

        /// <summary>
        /// Change the <see cref="CoreState"/>.
        /// </summary>
        /// <param name="state">The new state.</param>
        internal void ChangeCoreState(CoreState state)
        {
            CoreState = state;
            CoreStateChanged?.Invoke(this, state);
        }

        /// <inheritdoc/>
        public ValueTask DisposeAsync()
        {
            return default;
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<ICoreMember> GetContextById(string id)
        {
            if (_musicBrainzClient != null && _artistHelperService != null)
            {
                // Check if the ID is an artist
                var artist = await _musicBrainzClient.Artists.GetAsync(id, RelationshipQueries.Artists);
                if (artist != null)
                {
                    var totalTracksForArtist = await _artistHelperService.GetTotalTracksCount(artist);

                    yield return new MusicBrainzCoreArtist(this, artist, totalTracksForArtist);
                }

                // Check if the ID is a release
                var release = await _musicBrainzClient.Releases.GetAsync(id, RelationshipQueries.Releases);
                if (release != null)
                {
                    var releaseArtistData = release.Credits[0].Artist;
                    var totalTracksForArtist = await _artistHelperService.GetTotalTracksCount(releaseArtistData);
                    var releaseArtist = new MusicBrainzCoreArtist(this, releaseArtistData, totalTracksForArtist);

                    yield return new MusicBrainzCoreAlbum(this, release, releaseArtist);
                }

                // Check if the ID is a recording.
                var recordingData = await _musicBrainzClient.Recordings.GetAsync(id, RelationshipQueries.Recordings);

                // Iterating through retrieved releases.
                foreach (var item in recordingData.Releases)
                {
                    var releaseData = await _musicBrainzClient.Releases.GetAsync(item.Id);

                    // Iterating through retrieved release mediums.
                    foreach (var medium in releaseData.Media)
                    {
                        // Iterating through retrieved medium tracks to get the specific track for the id.
                        foreach (var track in medium.Tracks)
                        {
                            if (track.Recording.Id == id)
                            {
                                var artistData = releaseData.Credits[0].Artist;
                                var totalTracksForArtist = await _artistHelperService.GetTotalTracksCount(artistData);
                                var artistForTrackAlbum = new MusicBrainzCoreArtist(this, artistData, totalTracksForArtist);

                                var albumForTrack = new MusicBrainzCoreAlbum(this, releaseData, artistForTrackAlbum);

                                yield return new MusicBrainzCoreTrack(this, track, albumForTrack, medium.Position);
                            }
                        }
                    }
                }
            }
        }

        private async Task<ICoreSearchResults> GetSearchResultsAsync(string query)
        {
            if (_musicBrainzClient != null)
            {
                var recordings = await _musicBrainzClient.Recordings.SearchAsync($"*", 1);
                var releases = await _musicBrainzClient.Releases.SearchAsync("*", 1);
                var artists = await _musicBrainzClient.Artists.SearchAsync("*", 1);

                var results = new MusicBrainzCoreSearchResults(this, query)
                {
                    TotalTracksCount = recordings.Count,
                    TotalAlbumItemsCount = releases.Count,
                    TotalArtistItemsCount = artists.Count,
                    TotalPlaylistItemsCount = 0,
                    TotalChildrenCount = 0,
                };

                return results;
            }
            else
            {
                return new MusicBrainzCoreSearchResults(this, query);
            }
        }

        private bool _configured = false;

        /// <inheritdoc/>
        public Task InitAsync(IServiceCollection services)
        {
            ChangeCoreState(CoreState.Loading);

            if (!(CoreConfig is MusicBrainzCoreConfig coreConfig))
                return Task.CompletedTask;

            coreConfig.ConfigureServices(services);

            if (!_configured)
            {
                ChangeCoreState(CoreState.ConfigRequested);
                _configured = true;
                return Task.CompletedTask;
            }

            _musicBrainzClient = this.GetService<MusicBrainzClient>();
            _artistHelperService = this.GetService<MusicBrainzArtistHelpersService>();

            //var recordings = await _musicBrainzClient.Recordings.SearchAsync($"*", 1);
            //var releases = await _musicBrainzClient.Releases.SearchAsync("*", 1);
            //var artists = await _musicBrainzClient.Artists.SearchAsync("*", 1);

            Library = new MusicBrainzLibrary(this)
            {
                // Temporarily limited to reduce memory usage in merged collection map.
                TotalTracksCount = 1000,
                TotalAlbumItemsCount = 1000,
                TotalArtistItemsCount = 1000,
                TotalPlaylistItemsCount = 0,
                TotalChildrenCount = 0,
            };

            CoreState = CoreState.Loaded;
            CoreStateChanged?.Invoke(this, CoreState);

            Devices.Add(new MusicBrainzCoreDevice(this, "TestDevice", true)); // Hardcoded for now.

            return Task.CompletedTask;
        }
    }
}
