using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Hqub.MusicBrainz.API;
using Hqub.MusicBrainz.Client;
using Microsoft.Extensions.DependencyInjection;
using StrixMusic.Core.MusicBrainz.Utils;
using StrixMusic.Sdk.Interfaces;
using Microsoft.Extensions.DependencyInjection;
namespace StrixMusic.Core.MusicBrainz.Services
{
    /// <summary>
    /// A caching service for web API
    /// </summary>
    public class MusicBrainzCacheService
    {
        private FileRequestCache _fileRequest;
        private MusicBrainzClient _musicBrainzClient;
        private readonly ICore _sourCore;
        /// <summary>
        /// Method to configure caching
        /// </summary>
        public void ConfigureCache(ICore _sourCore)
        {
            _fileRequest = new FileRequestCache(string.Empty);
            _musicBrainzClient = _sourCore.CoreConfig.Services.GetService<MusicBrainzClient>();
            _musicBrainzClient.Cache = _fileRequest;
        }
    }
}
