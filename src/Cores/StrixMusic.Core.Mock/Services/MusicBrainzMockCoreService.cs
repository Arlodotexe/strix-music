using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Hqub.MusicBrainz.API;
using StrixMusic.CoreInterfaces.Interfaces;

namespace StrixMusic.Core.Mock.Services
{
    /// <inheritdoc />
    public class MusicBrainzMockCoreService : IMockCoreService
    {
        private MusicBrainzClient _musicBrainzClient;

        /// <inheritdoc />
        public MusicBrainzMockCoreService()
        {
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
        }

        /// <inheritdoc />
        public IReadOnlyList<IArtist> GetArtist()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public IReadOnlyList<IAlbum> GetRelease()
        {
            throw new NotImplementedException();
        }
    }
}
