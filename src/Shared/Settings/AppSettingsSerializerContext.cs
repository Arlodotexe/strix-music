using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using StrixMusic.AppModels;

namespace StrixMusic.Settings
{
    /// <summary>
    /// Supplies type information for settings values in <see cref="MusicSourcesSettings"/>.
    /// </summary>
    [JsonSourceGenerationOptions(WriteIndented = true)]
    [JsonSerializable(typeof(bool))]
    [JsonSerializable(typeof(string))]
    [JsonSerializable(typeof(int))]
    [JsonSerializable(typeof(LocalStorageCoreSettings))]
    [JsonSerializable(typeof(OneDriveCoreSettings))]
    [JsonSerializable(typeof(ObservableCollection<string>))]
    [JsonSerializable(typeof(ObservableCollection<LocalStorageCoreSettings>))]
    [JsonSerializable(typeof(ObservableCollection<OneDriveCoreSettings>))]
    [JsonSerializable(typeof(StrixMusicShells))]
    [JsonSerializable(typeof(AdaptiveShells))]
    public partial class AppSettingsSerializerContext : JsonSerializerContext
    {
    }
}
