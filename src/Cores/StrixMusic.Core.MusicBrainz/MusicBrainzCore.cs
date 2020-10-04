using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hqub.MusicBrainz.API;
using OwlCore.Collections;
using StrixMusic.Core.MusicBrainz.Models;
using StrixMusic.Core.MusicBrainz.Services;
using StrixMusic.Core.MusicBrainz.Statics;
using StrixMusic.Sdk.Enums;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.Interfaces;

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
        private readonly MusicBrainzClient _musicBrainzClient;
        private readonly MusicBrainzArtistHelpersService _artistHelperService;

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
            Devices = new SynchronizedObservableCollection<IDevice>();
            RecentlyPlayed = new MusicBrainzRecentlyPlayed(this);
            Discoverables = new MusicBrainzDiscoverables(this);
            User = new MusicBrainzUser(this);

            _musicBrainzClient = this.GetService<MusicBrainzClient>();
            _artistHelperService = this.GetService<MusicBrainzArtistHelpersService>();
        }

        /// <inheritdoc/>
        public ICoreConfig CoreConfig { get; }

        /// <inheritdoc/>
        public CoreState CoreState { get; internal set; } = CoreState.Unloaded;

        /// <inheritdoc/>
        public string Name => "MusicBrainz";

        /// <inheritdoc/>
        public IUser User { get; }

        /// <inheritdoc/>
        public SynchronizedObservableCollection<IDevice> Devices { get; }

        /// <inheritdoc/>
        public ILibrary Library { get; private set; }

        /// <inheritdoc/>
        public IRecentlyPlayed RecentlyPlayed { get; }

        /// <inheritdoc/>
        public IDiscoverables Discoverables { get; }

        /// <inheritdoc/>
        public string InstanceId { get; }

        /// <inheritdoc/>
        public SynchronizedObservableCollection<IPlayable> Pins { get; } = new SynchronizedObservableCollection<IPlayable>();

        /// <inheritdoc/>
        public event EventHandler<CoreState>? CoreStateChanged;

        /// <inheritdoc/>
        public ValueTask DisposeAsync()
        {
            return default;
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<object> GetContextById(string? id)
        {
            // Check if the ID is an artist
            var artist = await _musicBrainzClient.Artists.GetAsync(id, RelationshipQueries.Artists);
            if (artist != null)
            {
                var totalTracksForArtist = await _artistHelperService.GetTotalTracksCount(artist);

                yield return new MusicBrainzArtist(this, artist, totalTracksForArtist);
            }

            // Check if the ID is a release
            var release = await _musicBrainzClient.Releases.GetAsync(id, RelationshipQueries.Releases);
            if (release != null)
            {
                var releaseArtistData = release.Credits[0].Artist;
                var totalTracksForArtist = await _artistHelperService.GetTotalTracksCount(releaseArtistData);
                var releaseArtist = new MusicBrainzArtist(this, releaseArtistData, totalTracksForArtist);

                yield return new MusicBrainzAlbum(this, release, releaseArtist);
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
                            var artistForTrackAlbum = new MusicBrainzArtist(this, artistData, totalTracksForArtist);

                            var albumForTrack = new MusicBrainzAlbum(this, releaseData, artistForTrackAlbum);

                            yield return new MusicBrainzTrack(this, track, albumForTrack, medium.Position);
                        }
                    }
                }
            }
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<string> GetSearchAutoCompleteAsync(string query)
        {
            return AsyncEnumerable.Empty<string>();
        }

        /// <inheritdoc/>
        public async Task<ISearchResults> GetSearchResultsAsync(string query)
        {
            var recordings = await _musicBrainzClient.Recordings.SearchAsync($"*", 1);
            var releases = await _musicBrainzClient.Releases.SearchAsync("*", 1);
            var artists = await _musicBrainzClient.Artists.SearchAsync("*", 1);

            var results = new MusicBrainzSearchResults(this, query)
            {
                TotalTracksCount = recordings.Count,
                TotalAlbumsCount = releases.Count,
                TotalArtistsCount = artists.Count,
                TotalPlaylistCount = 0,
                TotalChildrenCount = 0,
            };

            return results;
        }

        /// <inheritdoc/>
        public async Task InitAsync()
        {
            CoreState = CoreState.Loading;
            CoreStateChanged?.Invoke(this, CoreState);

            var recordings = await _musicBrainzClient.Recordings.SearchAsync($"*", 1);
            var releases = await _musicBrainzClient.Releases.SearchAsync("*", 1);
            var artists = await _musicBrainzClient.Artists.SearchAsync("*", 1);

            Library = new MusicBrainzLibrary(this)
            {
                TotalTracksCount = recordings.Count,
                TotalAlbumsCount = releases.Count,
                TotalArtistsCount = artists.Count,
                TotalPlaylistCount = 0,
                TotalChildrenCount = 0,
            };

            CoreState = CoreState.Loaded;
            CoreStateChanged?.Invoke(this, CoreState);
        }

        /// <inheritdoc/>
        public Task<bool> IsAddPinSupported(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc/>
        public Task<bool> IsRemovePinSupported(int index)
        {
            return Task.FromResult(false);
        }
    }
}
