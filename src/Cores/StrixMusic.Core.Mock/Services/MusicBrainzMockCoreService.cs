using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Hqub.MusicBrainz.API;
using Hqub.MusicBrainz.API.Entities;
using Hqub.MusicBrainz.API.Entities.Collections;
using StrixMusic.Core.Mock.Models;
using StrixMusic.CoreInterfaces.Interfaces;

namespace StrixMusic.Core.Mock.Services
{
    /// <inheritdoc />
    public class MusicBrainzMockCoreService : IMockCoreService
    {
        private static MusicBrainzMockCoreService _instance;
        private MusicBrainzClient _musicBrainzClient;

        /// <summary>
        /// Init MusicBrainzClient
        /// </summary>
        private MusicBrainzMockCoreService()
        {
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
            _musicBrainzClient = new MusicBrainzClient();
        }

        /// <inheritdoc />
        public static MusicBrainzMockCoreService GetInstance()
        {
            if (_instance == null)
                _instance = new MusicBrainzMockCoreService();
            return _instance;
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
                    MockDescription = c.TextRepresentation.Script
                };
            }).ToList();
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<ITrack>> GetTracks()
        {
            var releases = await _musicBrainzClient.Recordings.SearchAsync("*", 1000);
            return null;
        }

        /// <inheritdoc/>
        public async Task<MockLibrary> GetLibraryAsync()
        {
            return new MockLibrary()
            {
                AlbumJson = (List<MockAlbum>)await GetAlbumsAsync(),
                ArtistJson = (List<MockArtist>)await GetArtistsAsync(),
            };
        }
    }
}
