using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Hqub.MusicBrainz.API;
using Microsoft.Extensions.DependencyInjection;
using StrixMusic.Core.MusicBrainz.Utils;

namespace StrixMusic.Core.MusicBrainz.Services
{
    /// <summary>
    /// A caching service for web API
    /// </summary>
    public class MusicBrainzCacheService
    {
        /// <summary>
        /// Method to configure caching
        /// </summary>
        public void ConfigureCache(MusicBrainzClient musicBrainzClient)
        {
            throw new NotImplementedException();
        }
    }
}
