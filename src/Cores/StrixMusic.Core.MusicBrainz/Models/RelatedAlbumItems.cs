using System;
using System.Collections.Generic;
using Hqub.MusicBrainz.API;
using Hqub.MusicBrainz.API.Entities;
using OwlCore.Collections;
using StrixMusic.Core.MusicBrainz.Services;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.Interfaces;

namespace StrixMusic.Core.MusicBrainz.Models
{
    /// <inheritdoc />
    public class RelatedAlbumItems : MusicBrainzCollectionGroupBase
    {

        private readonly MusicBrainzClient _musicBrainzClient;
        private readonly Release _release;
        private readonly MusicBrainzArtistHelpersService _artistHelperService;

        /// <inheritdoc />
        public RelatedAlbumItems(ICore sourceCore, Release release)
            : base(sourceCore)
        {
            _release = release;

            // Probably not the best way to do this.
            Id = $"related{_release.Id}";
            Name = "Other discs in this set";

            _musicBrainzClient = sourceCore.GetService<MusicBrainzClient>();
            _artistHelperService = sourceCore.GetService<MusicBrainzArtistHelpersService>();
        }

        /// <inheritdoc />
        public sealed override string Id { get; protected set; }

        /// <inheritdoc />
        public override Uri? Url { get; protected set; }

        /// <inheritdoc />
        public sealed override string Name { get; protected set; }

        /// <inheritdoc />
        public override SynchronizedObservableCollection<IImage> Images { get; protected set; } = new SynchronizedObservableCollection<IImage>();

        /// <inheritdoc />
        public override string? Description { get; protected set; }

        /// <inheritdoc />
        public override int TotalAlbumsCount { get; internal set; }

        /// <inheritdoc />
        public override int TotalArtistsCount { get; internal set; }

        /// <inheritdoc />
        public override int TotalTracksCount { get; internal set; }

        /// <inheritdoc />
        public override int TotalPlaylistCount { get; internal set; }

        /// <inheritdoc />
        public override int TotalChildrenCount { get; internal set; }

        /// <inheritdoc />
        public override async IAsyncEnumerable<IAlbum> GetAlbumsAsync(int limit, int offset = 0)
        {
            var release = await _musicBrainzClient.Releases.GetAsync(_release.Id);
            var artistData = release.Credits[0].Artist;

            var totalTracksForArtist = await _artistHelperService.GetTotalTracksCount(artistData);
            var artist = new MusicBrainzArtist(SourceCore, artistData, totalTracksForArtist);

            var mediums = release.Media.GetRange(offset, limit);
            foreach (var medium in mediums)
            {
                var album = new MusicBrainzAlbum(SourceCore, release, artist);

                yield return album;
            }
        }

        /// <inheritdoc />
        public override IAsyncEnumerable<IArtist> GetArtistsAsync(int limit, int offset = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override IAsyncEnumerable<IPlayableCollectionGroup> GetChildrenAsync(int limit, int offset = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override IAsyncEnumerable<IPlaylist> GetPlaylistsAsync(int limit, int offset = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override IAsyncEnumerable<ITrack> GetTracksAsync(int limit, int offset = 0)
        {
            throw new NotImplementedException();
        }
    }
}
