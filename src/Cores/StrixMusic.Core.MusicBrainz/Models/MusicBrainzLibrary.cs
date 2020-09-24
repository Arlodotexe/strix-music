using System;
using System.Collections.Generic;
using System.Linq;
using Hqub.MusicBrainz.API;
using OwlCore.Collections;
using StrixMusic.Core.MusicBrainz.Services;
using StrixMusic.Core.MusicBrainz.Statics;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.Interfaces;

namespace StrixMusic.Core.MusicBrainz.Models
{
    /// <summary>
    /// The lists of tracks in the dummy core's library.
    /// </summary>
    public class MusicBrainzLibrary : MusicBrainzCollectionGroupBase, ILibrary
    {
        private readonly MusicBrainzClient _musicBrainzClient;
        private readonly MusicBrainzArtistHelpersService _artistHelpersService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicBrainzLibrary"/> class.
        /// </summary>
        /// <param name="sourceCore"></param>
        public MusicBrainzLibrary(ICore sourceCore)
            : base(sourceCore)
        {
            _musicBrainzClient = SourceCore.GetService<MusicBrainzClient>();
            _artistHelpersService = SourceCore.GetService<MusicBrainzArtistHelpersService>();
        }

        /// <inheritdoc />
        public override string Id { get; protected set; } = "library";

        /// <inheritdoc />
        public override Uri? Url { get; protected set; } = null;

        /// <inheritdoc />
        public override string Name { get; protected set; } = "Library";

        /// <inheritdoc />
        public override SynchronizedObservableCollection<IImage> Images { get; protected set; } = new SynchronizedObservableCollection<IImage>();

        /// <inheritdoc />
        public override string? Description { get; protected set; } = null;

        /// <inheritdoc />
        public override int TotalChildrenCount { get; internal set; } = 0;

        /// <inheritdoc />
        public override int TotalArtistsCount { get; internal set; }

        /// <inheritdoc />
        public override int TotalAlbumsCount { get; internal set; }

        /// <inheritdoc />
        public override int TotalPlaylistCount { get; internal set; } = 0;

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
            var albumsList = await _musicBrainzClient.Releases.SearchAsync("*", limit, offset);
            var albums = albumsList.Items;

            foreach (var release in albums)
            {
                var artist = new MusicBrainzArtist(SourceCore, release.Credits[0].Artist)
                {
                    TotalTracksCount = await _artistHelpersService.GetTotalTracksCount(release.Credits[0].Artist),
                };

                foreach (var medium in release.Media)
                {
                    yield return new MusicBrainzAlbum(SourceCore, release, medium, artist);
                }
            }
        }

        /// <inheritdoc/>
        public override async IAsyncEnumerable<IArtist> GetArtistsAsync(int limit, int offset = 0)
        {
            var artists = await _musicBrainzClient.Artists.SearchAsync($"*&{RelationshipQueries.ArtistsQuery}", limit, offset);

            foreach (var artist in artists.Items)
            {
                yield return new MusicBrainzArtist(SourceCore, artist)
                {
                    TotalTracksCount = await _artistHelpersService.GetTotalTracksCount(artist),
                };
            }
        }

        /// <inheritdoc/>
        public override async IAsyncEnumerable<ITrack> GetTracksAsync(int limit, int offset = 0)
        {
            var recordings = await _musicBrainzClient.Recordings.SearchAsync($"*&{RelationshipQueries.RecordingsQuery}", limit, offset);

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
    }
}
