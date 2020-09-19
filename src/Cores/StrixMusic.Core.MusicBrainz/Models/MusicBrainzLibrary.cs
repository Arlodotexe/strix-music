using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hqub.MusicBrainz.API;
using StrixMusic.Core.MusicBrainz.Services;
using StrixMusic.Core.MusicBrainz.Statics;
using StrixMusic.Sdk.Events;
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
        public override IReadOnlyList<IImage> Images { get; protected set; } = new List<IImage>();

        /// <inheritdoc />
        public override string? Description { get; protected set; } = null;

        /// <inheritdoc />
        public override int TotalChildrenCount { get; protected set; } = 0;

        /// <inheritdoc />
        public override int TotalArtistsCount { get; protected set; }

        /// <inheritdoc />
        public override int TotalAlbumsCount { get; protected set; }

        /// <inheritdoc />
        public override int TotalPlaylistCount { get; protected set; } = 0;

        /// <inheritdoc />
        public override int TotalTracksCount { get; protected set; }

        /// <inheritdoc />
        public override event EventHandler<CollectionChangedEventArgs<IAlbum>>? AlbumsChanged;

        /// <inheritdoc />
        public override event EventHandler<CollectionChangedEventArgs<IArtist>>? ArtistsChanged;

        /// <inheritdoc />
        public override event EventHandler<CollectionChangedEventArgs<IPlayableCollectionGroup>>? ChildrenChanged;

        /// <inheritdoc />
        public override event EventHandler<CollectionChangedEventArgs<IPlaylist>>? PlaylistsChanged;

        /// <inheritdoc />
        public override event EventHandler<CollectionChangedEventArgs<ITrack>>? TracksChanged;

        /// <inheritdoc/>
        public override Task<IReadOnlyList<IPlayableCollectionGroup>> PopulateChildrenAsync(int limit, int offset = 0)
        {
            return Task.FromResult<IReadOnlyList<IPlayableCollectionGroup>>(new List<IPlayableCollectionGroup>());
        }

        /// <inheritdoc/>
        public override Task<IReadOnlyList<IPlaylist>> PopulatePlaylistsAsync(int limit, int offset = 0)
        {
            return Task.FromResult<IReadOnlyList<IPlaylist>>(new List<IPlaylist>());
        }

        /// <inheritdoc/>
        public override async Task<IReadOnlyList<IAlbum>> PopulateAlbumsAsync(int limit, int offset = 0)
        {
            var albumsList = await _musicBrainzClient.Releases.SearchAsync("*", limit, offset);
            var albums = albumsList.Items;

            var returnData = new List<IAlbum>();

            foreach (var release in albums)
            {
                var artist = new MusicBrainzArtist(SourceCore, release.Credits[0].Artist)
                {
                    TotalTracksCount = await _artistHelpersService.GetTotalTracksCount(release.Credits[0].Artist),
                };

                returnData.AddRange(release.Media.Select(medium => new MusicBrainzAlbum(SourceCore, release, medium, artist)));
            }

            var addedItems = returnData.Select((x, index) => new CollectionChangedEventArgsItem<IAlbum>(x, index));

            AlbumsChanged?.Invoke(this, new CollectionChangedEventArgs<IAlbum>(addedItems.ToArray(), null));

            SourceAlbums.AddRange(returnData);

            return returnData;
        }

        /// <inheritdoc/>
        public override async Task<IReadOnlyList<IArtist>> PopulateArtistsAsync(int limit, int offset = 0)
        {
            var artists = await _musicBrainzClient.Artists.SearchAsync($"*&{RelationshipQueries.ArtistsQuery}", limit, offset);

            var returnData = new List<IArtist>();

            foreach (var artist in artists.Items)
            {
                returnData.Add(new MusicBrainzArtist(SourceCore, artist)
                {
                    TotalTracksCount = await _artistHelpersService.GetTotalTracksCount(artist),
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
            var recordings = await _musicBrainzClient.Recordings.SearchAsync($"*&{RelationshipQueries.RecordingsQuery}", limit, offset);

            List<ITrack> returnData = new List<ITrack>();

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
