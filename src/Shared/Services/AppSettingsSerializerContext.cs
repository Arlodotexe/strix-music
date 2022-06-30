using System.Collections.Generic;
using System.Text.Json.Serialization;
using StrixMusic.Sdk.AdapterModels;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Sdk.Services;

namespace StrixMusic.Services
{
    /// <summary>
    /// Supplies type information for settings values in <see cref="AppSettings"/>.
    /// </summary>
    [JsonSourceGenerationOptions(WriteIndented = true)]
    [JsonSerializable(typeof(bool))]
    [JsonSerializable(typeof(string))]
    [JsonSerializable(typeof(List<string>))]
    [JsonSerializable(typeof(SiblingCollectionPlaybackPreferences))]
    [JsonSerializable(typeof(Dictionary<string, CoreMetadata>))]
    [JsonSerializable(typeof(MergedCollectionSorting))]
    internal partial class AppSettingsSerializerContext : JsonSerializerContext
    {
    }
}
