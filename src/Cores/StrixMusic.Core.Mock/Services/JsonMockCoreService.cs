using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StrixMusic.Core.Mock.Deserialization;
using StrixMusic.Core.Mock.Models;
using StrixMusic.CoreInterfaces.Interfaces;

namespace StrixMusic.Core.Mock.Services
{

    /// <summary>
    /// Service to get mock json data
    /// </summary>
    public class JsonMockCoreService : IMockCoreDataService
    {
        private static JsonMockCoreService _instance;

        private JsonMockCoreService()
        {
        }

        /// <inheritdoc />
        public static JsonMockCoreService GetInstance()
        {
            if (_instance == null)
                _instance = new JsonMockCoreService();
            return _instance;
        }

        /// <inheritdoc/>
        public Task<IReadOnlyList<IAlbum>> GetAlbumsAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<IReadOnlyList<IArtist>> GetArtistsAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public async Task<MockLibrary> GetLibraryAsync()
        {
            try
            {
                var resource = Extentions.GetManifestResourceString(Assembly.GetExecutingAssembly(), "StrixMusic.Core.Mock.Library.json");
                var lib = JsonConvert.DeserializeObject<MockLibrary>(resource);
                return lib;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
