using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hqub.MusicBrainz.API;
using Microsoft.Extensions.DependencyInjection;
using OwlCore.Collections;
using OwlCore.Events;
using OwlCore.Extensions;
using StrixMusic.Core.MusicBrainz.Models;
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
            Library = new MusicBrainzCoreLibrary(this);
            Devices = new SynchronizedObservableCollection<ICoreDevice>();
            RecentlyPlayed = new MusicBrainzCoreRecentlyPlayed(this);
            Discoverables = new MusicBrainzCoreDiscoverables(this);
            User = new MusicBrainzCoreUser(this);
        }

        /// <inheritdoc />
        public string InstanceDescriptor => "Mock data library.";

        /// <inheritdoc/>
        public ICoreConfig CoreConfig { get; }

        /// <inheritdoc />
        public ICore SourceCore => this;

        /// <inheritdoc/>
        public CoreState CoreState { get; internal set; } = CoreState.Unloaded;

        /// <inheritdoc/>
        public ICoreUser User { get; }

        /// <inheritdoc/>
        public IReadOnlyList<ICoreDevice> Devices { get; }

        /// <inheritdoc/>
        public ICoreLibrary Library { get; private set; }

        /// <inheritdoc />
        public ICoreSearch? Search { get; }

        /// <inheritdoc/>
        public ICoreRecentlyPlayed? RecentlyPlayed { get; }

        /// <inheritdoc/>
        public ICoreDiscoverables? Discoverables { get; }

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

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ICoreDevice>? DevicesChanged;

        /// <inheritdoc />
        public event EventHandler<string>? InstanceDescriptorChanged;

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
        public async ValueTask DisposeAsync()
        {
            if (RecentlyPlayed != null)
                await RecentlyPlayed.DisposeAsync();

            if (Pins != null)
                await Pins.DisposeAsync();

            if (Discoverables != null)
                await Discoverables.DisposeAsync();

            if (Search != null)
                await Search.DisposeAsync();

            await Devices.InParallel(x => x.DisposeAsync().AsTask());

            await Library.DisposeAsync();
        }

        /// <inheritdoc/>
        public Task<ICoreMember?> GetContextById(string id)
        {
            throw new NotImplementedException();
            /*if (_musicBrainzClient != null && _artistHelperService != null)
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
                    yield return new MusicBrainzCoreAlbum(this, release);
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
                                var albumForTrack = new MusicBrainzCoreAlbum(this, releaseData);

                                yield return new MusicBrainzCoreTrack(this, track, albumForTrack, medium.Position);
                            }
                        }
                    }
                }
            }*/
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
        public async Task InitAsync(IServiceCollection services)
        {
            ChangeCoreState(CoreState.Loading);

            if (!(CoreConfig is MusicBrainzCoreConfig coreConfig))
                return;

            // This was for testing purposes, and is now disabled.
            if (!_configured)
            {
                await coreConfig.SetupConfigurationServices(services);
                _configured = true;

                ChangeCoreState(CoreState.NeedsSetup);
                return;
            }

            await coreConfig.ConfigureServices(services);

            _musicBrainzClient = this.GetService<MusicBrainzClient>();

            //var recordings = await _musicBrainzClient.Recordings.SearchAsync($"*", 1);
            //var releases = await _musicBrainzClient.Releases.SearchAsync("*", 1);
            //var artists = await _musicBrainzClient.Artists.SearchAsync("*", 1);
            Library = new MusicBrainzCoreLibrary(this)
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
        }
    }
}
