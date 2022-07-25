using System.Text.Json.Serialization;

namespace StrixMusic.Cores.LocalFiles.Settings
{
    /// <summary>
    /// Supplies type information for settings values in <see cref="LocalFilesCoreSettings"/>.
    /// </summary>
    [JsonSourceGenerationOptions(WriteIndented = true)]
    [JsonSerializable(typeof(bool))]
    [JsonSerializable(typeof(string))]
    internal partial class FilesCoreSettingsSerializerContext : JsonSerializerContext
    {
    }
}
