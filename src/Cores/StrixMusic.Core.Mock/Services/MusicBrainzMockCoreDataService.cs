using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Hqub.MusicBrainz.API;
using Hqub.MusicBrainz.API.Entities;
using Hqub.MusicBrainz.API.Entities.Collections;
using Microsoft.Extensions.DependencyInjection;
using StrixMusic.Core.Mock.Models;
using StrixMusic.CoreInterfaces.Interfaces;
using StrixMusic.Services.Settings;

namespace StrixMusic.Core.Mock.Services
{
    /// <inheritdoc />
    public class MusicBrainzMockCoreDataService : SettingsServiceBase, IMockCoreDataService
    {
        private static MusicBrainzMockCoreDataService _instance;
        private MusicBrainzClient _musicBrainzClient;

        /// <inheritdoc/>
        public override string Id { get; }

        /// <inheritdoc/>
        public IServiceCollection Services => throw new NotImplementedException();

        /// <summary>
        /// Init MusicBrainzClient
        /// </summary>
        public MusicBrainzMockCoreDataService(string instanceId)
        {
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
            _musicBrainzClient = new MusicBrainzClient();
            Id = instanceId;
        }


        /// <inheritdoc />
        public async Task<IReadOnlyList<IArtist>> GetArtistsAsync()
        {
            var lst = new List<IArtist>();
            var artistList = await _musicBrainzClient.Artists.SearchAsync("*", 1000);
            return artistList.Items.Select(c =>
             {
                 return new MockArtist()
                 {
                     MockName = c.Name,
                     Id = c.Id,
                 };
             }).ToList();
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<IAlbum>> GetAlbumsAsync()
        {
            var releases = await _musicBrainzClient.Releases.SearchAsync("*", 1000);
            return releases.Items.Select(c =>
            {
                return new MockAlbum()
                {
                    MockId = c.Id,
                    NameJson = c.Title,
                    MockDescription = c.TextRepresentation.Script,
                };
            }).ToList();
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<ITrack>> GetTracksAsync()
        {
            var recordings = await _musicBrainzClient.Recordings.SearchAsync("*", 1000);
            return recordings.Items.Select(c =>
            {
                return new MockTrack()
                {
                    TitleJson = c.Title,
                    Id = c.Id,
                };
            }).ToList();
        }

        /// <inheritdoc/>
        public async Task<MockLibrary> GetLibraryAsync()
        {
            return new MockLibrary()
            {
                AlbumJson = (List<MockAlbum>)await GetAlbumsAsync(),
                ArtistJson = (List<MockArtist>)await GetArtistsAsync(),
                TracksJson = (List<MockTrack>)await GetTracksAsync(),
        };
    }
}
}
