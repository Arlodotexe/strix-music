using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StrixMusic.Core.MusicBrainz.Models;

namespace StrixMusic.Core.MusicBrainz.Utils
{
    /// <summary>
    /// Deserializes the JSON library, then hooks references up by Ids.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Gets an Embedded Resource as a <see cref="string"/>.
        /// </summary>
        /// <param name="assembly">The <see cref="Assembly"/> containing the resource.</param>
        /// <param name="path">The path of the Resource within the <see cref="Assembly"/>.</param>
        /// <returns>The resorce as a <see cref="string"/></returns>
        public static string GetManifestResourceString(this Assembly assembly, string path)
        {
            using var stream = assembly.GetManifestResourceStream(path);
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd().Trim();
        }
    }
}
