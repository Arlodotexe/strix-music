using System;
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
    [JsonSerializable(typeof(Uri))]
    [JsonSerializable(typeof(Version))]
    [JsonSerializable(typeof(SiblingCollectionPlaybackPreferences))]
    [JsonSerializable(typeof(CoreMetadata))]
    [JsonSerializable(typeof(MergedCollectionSorting))]
    [JsonSerializable(typeof(SiblingCollectionPlaybackBehavior))]
    [JsonSerializable(typeof(List<string>))]
    [JsonSerializable(typeof(Dictionary<string, CoreMetadata>))]
    internal partial class AppSettingsSerializerContext : JsonSerializerContext
    {
    }
}
