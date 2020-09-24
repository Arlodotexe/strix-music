using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Hqub.MusicBrainz.API;
using Hqub.MusicBrainz.API.Entities;
using OwlCore.Collections;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.Interfaces;

namespace StrixMusic.Core.MusicBrainz.Models
{
    /// <inheritdoc />
    public class RelatedAlbumItems : MusicBrainzCollectionGroupBase
    {

        private readonly MusicBrainzClient _musicBrainzClient;
        private readonly Release _release;

        /// <inheritdoc />
        public RelatedAlbumItems(ICore sourceCore, Release release)
            : base(sourceCore)
        {
            _release = release;
            TotalAlbumsCount = release.Media.Count;
            _musicBrainzClient = sourceCore.GetService<MusicBrainzClient>();
        }

        /// <inheritdoc />
        public override string Id { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }

        /// <inheritdoc />
        public override Uri? Url { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }

        /// <inheritdoc />
        public override string Name { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }

        /// <inheritdoc />
        public override SynchronizedObservableCollection<IImage> Images { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }

        /// <inheritdoc />
        public override string? Description { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }

        /// <inheritdoc />
        public override int TotalAlbumsCount { get => throw new NotImplementedException(); internal set => throw new NotImplementedException(); }

        /// <inheritdoc />
        public override int TotalArtistsCount { get => throw new NotImplementedException(); internal set => throw new NotImplementedException(); }

        /// <inheritdoc />
        public override int TotalTracksCount { get => throw new NotImplementedException(); internal set => throw new NotImplementedException(); }

        /// <inheritdoc />
        public override int TotalPlaylistCount { get => throw new NotImplementedException(); internal set => throw new NotImplementedException(); }

        /// <inheritdoc />
        public override int TotalChildrenCount { get => throw new NotImplementedException(); internal set => throw new NotImplementedException(); }

        /// <inheritdoc />
        public async override IAsyncEnumerable<IAlbum> GetAlbumsAsync(int limit, int offset = 0)
        {
            var release = await _musicBrainzClient.Releases.GetAsync(_release.Id);
            var artist = new MusicBrainzArtist(SourceCore, release.Credits[0].Artist);

            var mediums = release.Media.GetRange(offset, limit);
            foreach (var medium in mediums)
            {
                var album = new MusicBrainzAlbum(SourceCore, release, medium, artist);
                Albums.Add(album);
                IsRemoveAlbumSupportedMap.Add(false);

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

        /// <inheritdoc />
        public override Task PopulateMoreAlbumsAsync(int limit)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override Task PopulateMoreArtistsAsync(int limit)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override Task PopulateMoreChildrenAsync(int limit)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override Task PopulateMorePlaylistsAsync(int limit)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override Task PopulateMoreTracksAsync(int limit)
        {
            throw new NotImplementedException();
        }
    }
}
