using Hqub.MusicBrainz.API;
using StrixMusic.Core.MusicBrainz.Services;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StrixMusic.Sdk.Events;
using IArtist = StrixMusic.Sdk.Interfaces.IArtist;

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
        public override IReadOnlyList<IImage> Images { get; protected set; } = new List<IImage>();

        /// <inheritdoc />
        public override string? Description { get; protected set; } = null;

        /// <inheritdoc />
        public override int TotalChildrenCount { get; protected set; }

        /// <inheritdoc />
        public override int TotalPlaylistCount { get; protected set; }

        /// <inheritdoc />
        public override int TotalArtistsCount { get; protected set; }

        /// <inheritdoc />
        public override int TotalAlbumsCount { get; protected set; }

        /// <inheritdoc />
        public override int TotalTracksCount { get; protected set; }

        /// <inheritdoc />
        public override event EventHandler<CollectionChangedEventArgs<ITrack>>? TracksChanged;

        /// <inheritdoc />
        public override event EventHandler<CollectionChangedEventArgs<IPlaylist>>? PlaylistsChanged;

        /// <inheritdoc />
        public override event EventHandler<CollectionChangedEventArgs<IPlayableCollectionGroup>>? ChildrenChanged;

        /// <inheritdoc />
        public override event EventHandler<CollectionChangedEventArgs<IArtist>>? ArtistsChanged;

        /// <inheritdoc />
        public override event EventHandler<CollectionChangedEventArgs<IAlbum>>? AlbumsChanged;

        /// <inheritdoc/>
        public override Task<IReadOnlyList<IPlayableCollectionGroup>> PopulateChildrenAsync(int limit, int offset = 0)
        {
            return Task.FromResult(Children);
        }

        /// <inheritdoc/>
        public override Task<IReadOnlyList<IPlaylist>> PopulatePlaylistsAsync(int limit, int offset = 0)
        {
            return Task.FromResult(Playlists);
        }

        /// <inheritdoc/>
        public override async Task<IReadOnlyList<IAlbum>> PopulateAlbumsAsync(int limit, int offset = 0)
        {
            var releases = await _musicBrainzClient.Releases.SearchAsync(_query, limit, offset);

            var returnData = new List<IAlbum>();

            foreach (var release in releases)
            {
                var artist = new MusicBrainzArtist(SourceCore, release.Credits[0].Artist)
                {
                    TotalTracksCount = await _artistHelpersService.GetTotalTracksCount(release.Credits[0].Artist),
                };

                returnData.AddRange(release.Media.Select(x => new MusicBrainzAlbum(SourceCore, release, x, artist)));
            }

            var addedItems = returnData.Select((x, index) => new CollectionChangedEventArgsItem<IAlbum>(x, index));

            AlbumsChanged?.Invoke(this, new CollectionChangedEventArgs<IAlbum>(addedItems.ToArray(), null));

            SourceAlbums.AddRange(returnData);

            return returnData;
        }

        /// <inheritdoc/>
        public override async Task<IReadOnlyList<IArtist>> PopulateArtistsAsync(int limit, int offset = 0)
        {
            var artists = await _musicBrainzClient.Artists.SearchAsync(_query, limit, offset);

            var returnData = new List<IArtist>();

            foreach (var item in artists)
            {
                returnData.Add(new MusicBrainzArtist(SourceCore, item)
                {
                    TotalTracksCount = await _artistHelpersService.GetTotalTracksCount(item),
                });
            }

            var addedItems = returnData.Select((x, index) => new CollectionChangedEventArgsItem<IArtist>(x, index));

            ArtistsChanged?.Invoke(this, new CollectionChangedEventArgs<IArtist>(addedItems.ToArray(), null));

            SourceArtists.AddRange(returnData);

            return returnData;
        }

        /// <inheritdoc/>
        public override async Task<IReadOnlyList<ITrack>> PopulateTracksAsync(int limit, int offset = 0)
        {
            var recordings = await _musicBrainzClient.Recordings.SearchAsync(_query, limit, offset);

            var returnData = new List<ITrack>();

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
                            returnData.Add(new MusicBrainzTrack(SourceCore, track, new MusicBrainzAlbum(SourceCore, release, medium, artist)));
                        }
                    }
                }
            }

            var addedItems = returnData.Select((x, index) => new CollectionChangedEventArgsItem<ITrack>(x, index));

            TracksChanged?.Invoke(this, new CollectionChangedEventArgs<ITrack>(addedItems.ToArray(), null));

            SourceTracks.AddRange(returnData);

            return returnData;
        }
    }
}
