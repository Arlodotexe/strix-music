using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Hqub.MusicBrainz.API;
using StrixMusic.Core.MusicBrainz.Services;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.Interfaces;

namespace StrixMusic.Core.MusicBrainz.Models
{
    /// <inheritdoc cref="ISearchResults" />
    public class MusicBrainzSearchResults : MusicBrainzCollectionGroupBase, ISearchResults
    {
        private readonly MusicBrainzClient _musicBrainzClient;
        private readonly string _query;
        private readonly MusicBrainzArtistHelpersService _artistHelpersService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicBrainzSearchResults"/> class.
        /// </summary>
        /// <param name="sourceCore">The core that created this object.</param>
        /// <param name="query">The search query for these results.</param>
        public MusicBrainzSearchResults(ICore sourceCore, string query)
            : base(sourceCore)
        {
            _musicBrainzClient = SourceCore.GetService<MusicBrainzClient>();
            _artistHelpersService = SourceCore.GetService<MusicBrainzArtistHelpersService>();
            Id = query.GetHashCode().ToString();

            _query = query;
        }

        /// <inheritdoc />
        public sealed override string Id { get; protected set; }

        /// <inheritdoc />
        public override Uri? Url { get; protected set; } = null;

        /// <inheritdoc />
        public override string Name { get; protected set; } = "Search Results";

        /// <inheritdoc />
        public override ObservableCollection<IImage> Images { get; protected set; } = new ObservableCollection<IImage>();

        /// <inheritdoc />
        public override string? Description { get; protected set; } = null;

        /// <inheritdoc />
        public override int TotalChildrenCount { get; internal set; }

        /// <inheritdoc />
        public override int TotalPlaylistCount { get; internal set; }

        /// <inheritdoc />
        public override int TotalArtistsCount { get; internal set; }

        /// <inheritdoc />
        public override int TotalAlbumsCount { get; internal set; }

        /// <inheritdoc />
        public override int TotalTracksCount { get; internal set; }

        /// <inheritdoc/>
        public override IAsyncEnumerable<IPlayableCollectionGroup> GetChildrenAsync(int limit, int offset = 0)
        {
            return AsyncEnumerable.Empty<IPlayableCollectionGroup>();
        }

        /// <inheritdoc/>
        public override IAsyncEnumerable<IPlaylist> GetPlaylistsAsync(int limit, int offset = 0)
        {
            return AsyncEnumerable.Empty<IPlaylist>();
        }

        /// <inheritdoc/>
        public override async IAsyncEnumerable<IAlbum> GetAlbumsAsync(int limit, int offset = 0)
        {
            var releases = await _musicBrainzClient.Releases.SearchAsync(_query, limit, offset);

            foreach (var release in releases)
            {
                var artist = new MusicBrainzArtist(SourceCore, release.Credits[0].Artist)
                {
                    TotalTracksCount = await _artistHelpersService.GetTotalTracksCount(release.Credits[0].Artist),
                };

                foreach (var albumData in release.Media)
                {
                    yield return new MusicBrainzAlbum(SourceCore, release, albumData, artist);
                }
            }
        }

        /// <inheritdoc/>
        public override async IAsyncEnumerable<IArtist> GetArtistsAsync(int limit, int offset = 0)
        {
            var artists = await _musicBrainzClient.Artists.SearchAsync(_query, limit, offset);

            foreach (var item in artists)
            {
                yield return new MusicBrainzArtist(SourceCore, item)
                {
                    TotalTracksCount = await _artistHelpersService.GetTotalTracksCount(item),
                };
            }
        }

        /// <inheritdoc/>
        public override async IAsyncEnumerable<ITrack> GetTracksAsync(int limit, int offset = 0)
        {
            var recordings = await _musicBrainzClient.Recordings.SearchAsync(_query, limit, offset);

            foreach (var recording in recordings)
            {
                foreach (var release in recording.Releases)
                {
                    var artist = new MusicBrainzArtist(SourceCore, release.Credits[0].Artist)
                    {
                        TotalTracksCount = await _artistHelpersService.GetTotalTracksCount(release.Credits[0].Artist),
                    };

                    foreach (var medium in release.Media)
                    {
                        foreach (var track in medium.Tracks)
                        {
                            yield return new MusicBrainzTrack(SourceCore, track, new MusicBrainzAlbum(SourceCore, release, medium, artist));
                        }
                    }
                }
            }
        }

        /// <inheritdoc/>
        public override async Task PopulateMoreTracksAsync(int limit)
        {
            var offset = Tracks.Count;
            await foreach (var item in GetTracksAsync(limit, offset))
            {
                IsRemoveTrackSupportedMap.Add(false);
                Tracks.Add(item);
            }
        }

        /// <inheritdoc/>
        public override async Task PopulateMoreAlbumsAsync(int limit)
        {
            var offset = Albums.Count;
            await foreach (var item in GetAlbumsAsync(limit, offset))
            {
                IsRemoveAlbumSupportedMap.Add(false);
                Albums.Add(item);
            }
        }

        /// <inheritdoc/>
        public override Task PopulateMorePlaylistsAsync(int limit)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public override Task PopulateMoreChildrenAsync(int limit)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public override async Task PopulateMoreArtistsAsync(int limit)
        {
            var offset = Artists.Count;
            await foreach (var item in GetArtistsAsync(limit, offset))
            {
                IsRemoveArtistSupportedMap.Add(false);
                Artists.Add(item);
            }
        }
    }
}
