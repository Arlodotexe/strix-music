using System;
using System.Collections.Generic;
using System.Linq;
using Hqub.MusicBrainz.API;
using OwlCore.Collections;
using StrixMusic.Core.MusicBrainz.Services;
using StrixMusic.Sdk.Core.Data;
using StrixMusic.Sdk.Extensions;

namespace StrixMusic.Core.MusicBrainz.Models
{
    /// <summary>
    /// A MusicBrainz implementation of <see cref="ICoreSearchResults"/>.
    /// </summary>
    public class MusicBrainzCoreSearchResults : MusicBrainzCollectionGroupBase, ICoreSearchResults
    {
        private readonly MusicBrainzClient _musicBrainzClient;
        private readonly string _query;
        private readonly MusicBrainzArtistHelpersService _artistHelpersService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicBrainzCoreSearchResults"/> class.
        /// </summary>
        /// <param name="sourceCore">The core that created this object.</param>
        /// <param name="query">The search query for these results.</param>
        public MusicBrainzCoreSearchResults(ICore sourceCore, string query)
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
        public override SynchronizedObservableCollection<IImage> Images { get; protected set; } = new SynchronizedObservableCollection<IImage>();

        /// <inheritdoc />
        public override string? Description { get; protected set; } = null;

        /// <inheritdoc />
        public override int TotalChildrenCount { get; internal set; }

        /// <inheritdoc />
        public override int TotalPlaylistItemsCount { get; internal set; }

        /// <inheritdoc />
        public override int TotalArtistItemsCount { get; internal set; }

        /// <inheritdoc />
        public override int TotalAlbumItemsCount { get; internal set; }

        /// <inheritdoc />
        public override int TotalTracksCount { get; internal set; }

        /// <inheritdoc/>
        public override IAsyncEnumerable<IPlayableCollectionGroup> GetChildrenAsync(int limit, int offset = 0)
        {
            return AsyncEnumerable.Empty<IPlayableCollectionGroup>();
        }

        /// <inheritdoc/>
        public override IAsyncEnumerable<IPlaylistCollectionItem> GetPlaylistItemsAsync(int limit, int offset)
        {
            return AsyncEnumerable.Empty<ICorePlaylist>();
        }

        /// <inheritdoc/>
        public override async IAsyncEnumerable<ICoreAlbumCollectionItem> GetAlbumItemsAsync(int limit, int offset)
        {
            var releases = await _musicBrainzClient.Releases.SearchAsync(_query, limit, offset);

            foreach (var release in releases.Items)
            {
                var artistForRelease = release.Credits[0].Artist;
                var totalTracksCount = await _artistHelpersService.GetTotalTracksCount(artistForRelease);

                var artist = new MusicBrainzCoreArtist(SourceCore, artistForRelease, totalTracksCount);

                yield return new MusicBrainzCoreAlbum(SourceCore, release, artist);
            }
        }

        /// <inheritdoc/>
        public override async IAsyncEnumerable<IArtistCollectionItem> GetArtistsAsync(int limit, int offset)
        {
            var artists = await _musicBrainzClient.Artists.SearchAsync(_query, limit, offset);

            foreach (var item in artists.Items)
            {
                var totalTracksForArtist = await _artistHelpersService.GetTotalTracksCount(item);
                yield return new MusicBrainzCoreArtist(SourceCore, item, totalTracksForArtist);
            }
        }

        /// <inheritdoc/>
        public override async IAsyncEnumerable<ICoreTrack> GetTracksAsync(int limit, int offset = 0)
        {
            var recordings = await _musicBrainzClient.Recordings.SearchAsync(_query, limit, offset);

            foreach (var recording in recordings.Items)
            {
                foreach (var release in recording.Releases)
                {
                    var artistData = release.Credits[0].Artist;
                    var totalTracksForArtist = await _artistHelpersService.GetTotalTracksCount(artistData);
                    var artist = new MusicBrainzCoreArtist(SourceCore, artistData, totalTracksForArtist);

                    foreach (var medium in release.Media)
                    {
                        foreach (var track in medium.Tracks)
                        {
                            var album = new MusicBrainzCoreAlbum(SourceCore, release, artist);
                            yield return new MusicBrainzCoreTrack(SourceCore, track, album, medium.Position);
                        }
                    }
                }
            }
        }
    }
}
