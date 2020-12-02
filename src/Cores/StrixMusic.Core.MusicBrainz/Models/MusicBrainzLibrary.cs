using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hqub.MusicBrainz.API;
using Microsoft.Toolkit.Diagnostics;
using StrixMusic.Core.MusicBrainz.Services;
using StrixMusic.Core.MusicBrainz.Statics;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Extensions;

namespace StrixMusic.Core.MusicBrainz.Models
{
    /// <summary>
    /// A MusicBrainz implementation of <see cref="ICoreLibrary"/>.
    /// </summary>
    public class MusicBrainzLibrary : MusicBrainzCollectionGroupBase, ICoreLibrary
    {
        private readonly MusicBrainzClient? _musicBrainzClient;
        private readonly MusicBrainzArtistHelpersService? _artistHelpersService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicBrainzLibrary"/> class.
        /// </summary>
        /// <param name="sourceCore"></param>
        public MusicBrainzLibrary(ICore sourceCore)
            : base(sourceCore)
        {
            _musicBrainzClient = SourceCore.GetServiceSafe<MusicBrainzClient>();

            if (_musicBrainzClient != null)
                _artistHelpersService = SourceCore.GetServiceSafe<MusicBrainzArtistHelpersService>() ?? new MusicBrainzArtistHelpersService(_musicBrainzClient);
        }

        /// <inheritdoc />
        public override string Id { get; protected set; } = "library";

        /// <inheritdoc />
        public override Uri? Url { get; protected set; } = null;

        /// <inheritdoc />
        public override string Name { get; protected set; } = "Library";

        /// <inheritdoc />
        public override string? Description { get; protected set; } = null;

        /// <inheritdoc />
        public override int TotalChildrenCount { get; internal set; } = 0;

        /// <inheritdoc />
        public override int TotalArtistItemsCount { get; internal set; }

        /// <inheritdoc />
        public override int TotalAlbumItemsCount { get; internal set; }

        /// <inheritdoc />
        public override int TotalPlaylistItemsCount { get; internal set; } = 0;

        /// <inheritdoc />
        public override int TotalTracksCount { get; internal set; }

        /// <inheritdoc/>
        public override IAsyncEnumerable<ICorePlayableCollectionGroup> GetChildrenAsync(int limit, int offset = 0)
        {
            return AsyncEnumerable.Empty<ICorePlayableCollectionGroup>();
        }

        /// <inheritdoc/>
        public override async IAsyncEnumerable<ICorePlaylistCollectionItem> GetPlaylistItemsAsync(int limit, int offset)
        {
            await Task.CompletedTask;
            yield break;
        }

        /// <inheritdoc/>
        public override async IAsyncEnumerable<ICoreAlbumCollectionItem> GetAlbumItemsAsync(int limit, int offset)
        {
            Guard.IsNotNull(_musicBrainzClient, nameof(_musicBrainzClient));
            Guard.IsNotNull(_artistHelpersService, nameof(_artistHelpersService));

            var albumsList = await _musicBrainzClient.Releases.SearchAsync("*", limit, offset);
            var albums = albumsList.Items;

            foreach (var release in albums)
            {
                var releaseArtist = release.Credits[0].Artist;
                var totalTracksForArtist = await _artistHelpersService.GetTotalTracksCount(releaseArtist);
                var artist = new MusicBrainzCoreArtist(SourceCore, releaseArtist, totalTracksForArtist);

                yield return new MusicBrainzCoreAlbum(SourceCore, release, artist);
            }
        }

        /// <inheritdoc/>
        public override async IAsyncEnumerable<ICoreArtistCollectionItem> GetArtistItemsAsync(int limit, int offset)
        {
            Guard.IsNotNull(_musicBrainzClient, nameof(_musicBrainzClient));
            Guard.IsNotNull(_artistHelpersService, nameof(_artistHelpersService));

            var artists = await _musicBrainzClient.Artists.SearchAsync($"*", limit, offset);

            foreach (var artist in artists.Items)
            {
                int totalTracksForArtist = await _artistHelpersService.GetTotalTracksCount(artist, 50);

                yield return new MusicBrainzCoreArtist(SourceCore, artist, totalTracksForArtist);
            }
        }

        /// <inheritdoc/>
        public override async IAsyncEnumerable<ICoreTrack> GetTracksAsync(int limit, int offset = 0)
        {
            Guard.IsNotNull(_musicBrainzClient, nameof(_musicBrainzClient));
            Guard.IsNotNull(_artistHelpersService, nameof(_artistHelpersService));

            var recordings = await _musicBrainzClient.Recordings.SearchAsync("*", limit, offset);

            foreach (var recording in recordings.Items)
            {
                foreach (var release in recording.Releases)
                {
                    // The search query above doesn't include track data, so we have to get it ourselves.
                    var releaseData =
                        await _musicBrainzClient.Releases.GetAsync(release.Id, RelationshipQueries.Releases);

                    var totalTracksForArtist =
                        await _artistHelpersService.GetTotalTracksCount(recording.Credits[0].Artist);

                    var artist = new MusicBrainzCoreArtist(SourceCore, recording.Credits[0].Artist, totalTracksForArtist);

                    foreach (var medium in releaseData.Media)
                    {
                        foreach (var track in medium.Tracks)
                        {
                            if (track.Recording.Id == recording.Id)
                            {
                                var albumForTrack = new MusicBrainzCoreAlbum(SourceCore, release, artist);
                                yield return new MusicBrainzCoreTrack(SourceCore, track, albumForTrack, medium.Position);
                            }
                        }
                    }
                }
            }
        }
    }
}
