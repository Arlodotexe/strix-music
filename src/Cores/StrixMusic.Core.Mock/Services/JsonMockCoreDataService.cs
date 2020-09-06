using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using StrixMusic.Core.Mock.Deserialization;
using StrixMusic.Core.Mock.Models;
using StrixMusic.CoreInterfaces.Interfaces;
using StrixMusic.Services.Settings;

namespace StrixMusic.Core.Mock.Services
{

    /// <summary>
    /// Service to get mock json data
    /// </summary>
    public class JsonMockCoreDataService : IMockCoreDataService
    {
        /// <summary>
        /// Init
        /// </summary>
        public JsonMockCoreDataService(string instanceId)
        {
            Id = instanceId;
        }

        /// <inheritdoc/>
        public IServiceCollection Services => throw new NotImplementedException();

        /// <inheritdoc/>
        public string Id { get; }

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
