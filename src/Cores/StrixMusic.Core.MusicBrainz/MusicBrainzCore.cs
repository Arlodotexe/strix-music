using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hqub.MusicBrainz.API;
using OwlCore.Collections;
using StrixMusic.Core.MusicBrainz.Models;
using StrixMusic.Core.MusicBrainz.Statics;
using StrixMusic.Sdk.Enums;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.Interfaces;

namespace StrixMusic.Core.MusicBrainz
{
    /// <summary>
    /// Mock Core
    /// </summary>
    public class MusicBrainzCore : ICore
    {
        private readonly MusicBrainzClient _musicBrainzClient;

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
        public SynchronizedObservableCollection<bool> IsRemovePinSupportedMap { get; } = new SynchronizedObservableCollection<bool>();

        /// <inheritdoc/>
        public event EventHandler<CoreState>? CoreStateChanged;

        /// <inheritdoc/>
        public ValueTask DisposeAsync()
        {
            return default;
        }

        /// <inheritdoc/>
        public async Task<object?> GetContextById(string? id)
        {
            var artist = await _musicBrainzClient.Artists.GetAsync(id, RelationshipQueries.Artists);
            if (artist != null)
            {
                return new MusicBrainzArtist(this, artist);
            }

            // A list to include all albums for a release id.
            var albums = new List<MusicBrainzAlbum>();
            var release = await _musicBrainzClient.Releases.GetAsync(id, RelationshipQueries.Releases);
            if (release != null)
            {
                var releaseArtist = new MusicBrainzArtist(this, release.Credits[0].Artist);
                foreach (var medium in release.Media)
                {
                    var album = new MusicBrainzAlbum(this, release, medium, releaseArtist);
                    albums.Add(album);
                }

                return albums;
            }

            var releasesList =
                await _musicBrainzClient.Releases.SearchAsync("*", 100, 0);

            // Created to add all releases on MusicBrainz.
            var releases = releasesList.Items.ToList();

            // Iterating through each page for releases.
            for (var i = 100; i < releases.Count; i += 100)
            {
                var nextReleasesPage = await _musicBrainzClient.Releases.SearchAsync("*", 100, i);

                releases.AddRange(nextReleasesPage.Items);
            }

            // Iterating through retrieved releases.
            foreach (var releaseItem in releasesList)
            {
                // Iterating through retrieved release mediums.
                foreach (var medium in releaseItem.Media)
                {
                    // Iterating through retrieved medium tracks to get the specific track for the id.
                    foreach (var track in medium.Tracks)
                    {
                        if (track.Id == id)
                        {
                            var artistForTrack = new MusicBrainzArtist(this, releaseItem.Credits[0].Artist);
                            var albumForTrack = new MusicBrainzAlbum(this, releaseItem, medium, artistForTrack);
                            return new MusicBrainzTrack(this, track, albumForTrack);
                        }
                    }
                }
            }

            return null;
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
    }
}
