using System.Text.Json.Serialization;

namespace StrixMusic.Services
{
    /// <summary>
    /// Supplies type information for settings values in <see cref="AppSettings"/>.
    /// </summary>
    [JsonSourceGenerationOptions(WriteIndented = true)]
    [JsonSerializable(typeof(bool))]
    [JsonSerializable(typeof(string))]
    [JsonSerializable(typeof(LocalStorageCoreSettings))]
    [JsonSerializable(typeof(OneDriveCoreSettings))]
    [JsonSerializable(typeof(ShellSettings))]
    public partial class AppSettingsSerializerContext : JsonSerializerContext
    {
    }
}
