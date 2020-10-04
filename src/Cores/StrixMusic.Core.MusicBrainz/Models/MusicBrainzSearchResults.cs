using System;
using System.Collections.Generic;
using System.Linq;
using Hqub.MusicBrainz.API;
using OwlCore.Collections;
using StrixMusic.Core.MusicBrainz.Services;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.Interfaces;

namespace StrixMusic.Core.MusicBrainz.Models
{
    /// <summary>
    /// A MusicBrainz implementation of <see cref="ISearchResults"/>.
    /// </summary>
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
        public override SynchronizedObservableCollection<IImage> Images { get; protected set; } = new SynchronizedObservableCollection<IImage>();

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

            foreach (var release in releases.Items)
            {
                var artistForRelease = release.Credits[0].Artist;
                var totalTracksCount = await _artistHelpersService.GetTotalTracksCount(artistForRelease);

                var artist = new MusicBrainzArtist(SourceCore, artistForRelease, totalTracksCount);

                yield return new MusicBrainzAlbum(SourceCore, release, artist);
            }
        }

        /// <inheritdoc/>
        public override async IAsyncEnumerable<IArtist> GetArtistsAsync(int limit, int offset = 0)
        {
            var artists = await _musicBrainzClient.Artists.SearchAsync(_query, limit, offset);

            foreach (var item in artists.Items)
            {
                var totalTracksForArtist = await _artistHelpersService.GetTotalTracksCount(item);
                yield return new MusicBrainzArtist(SourceCore, item, totalTracksForArtist);
            }
        }

        /// <inheritdoc/>
        public override async IAsyncEnumerable<ITrack> GetTracksAsync(int limit, int offset = 0)
        {
            var recordings = await _musicBrainzClient.Recordings.SearchAsync(_query, limit, offset);

            foreach (var recording in recordings.Items)
            {
                foreach (var release in recording.Releases)
                {
                    var artistData = release.Credits[0].Artist;
                    var totalTracksForArtist = await _artistHelpersService.GetTotalTracksCount(artistData);
                    var artist = new MusicBrainzArtist(SourceCore, artistData, totalTracksForArtist);

                    foreach (var medium in release.Media)
                    {
                        foreach (var track in medium.Tracks)
                        {
                            var album = new MusicBrainzAlbum(SourceCore, release, artist);
                            yield return new MusicBrainzTrack(SourceCore, track, album, medium.Position);
                        }
                    }
                }
            }
        }
    }
}
