using System.Text.Json.Serialization;

namespace StrixMusic.Helpers.Quips
{
    /// <summary>
    /// A class for deserializing quip files.
    /// </summary>
    public class QuipFile
    {
        // TODO: Source and updates information

        /// <summary>
        /// An array of quip groups contained in the file.
        /// </summary>
        [JsonPropertyName("QuipGroups")]
        public QuipGroup[]? QuipGroups { get; set; }

        /// <summary>
        /// Gets or sets if the quip file comes with Strix-Music.
        /// </summary>
        [JsonPropertyName("Integrated")]
        public bool Integrated { get; set; }
    }
}
