using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StrixMusic.Core.Mock.Deserialization;
using StrixMusic.Core.Mock.Models;

namespace StrixMusic.Core.Mock.Deserialization
{
    /// <summary>
    /// Deserializes the JSON library, then hooks references up by Ids.
    /// </summary>
    public static class Deserializer
    {
        /// <summary>
        /// Deserializes and sets up the DummyCore <see cref="SerializedLibrary"/>.
        /// </summary>
        /// <returns>The <see cref="SerializedLibrary"/> of the DummyCore.</returns>
        public static async Task<SerializedLibrary> DeserializeLibrary()
        {
            try
            {
                var resource = GetManifestResourceString(Assembly.GetExecutingAssembly(), "StrixMusic.Core.Mock.Library.json");
                var lib = JsonConvert.DeserializeObject<SerializedLibrary>(resource);
                return lib;
            }
            catch(Exception ex)
            {
                return null;
            }
        }
        /// <summary>
        /// Gets an Embedded Resource as a <see cref="string"/>.
        /// </summary>
        /// <param name="assembly">The <see cref="Assembly"/> containing the resource.</param>
        /// <param name="path">The path of the Resource within the <see cref="Assembly"/>.</param>
        /// <returns>The resorce as a <see cref="string"/></returns>
        private static string GetManifestResourceString(this Assembly assembly, string path)
        {
            using var stream = assembly.GetManifestResourceStream(path);
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd().Trim();
        }
    }
}
