using System.Text.Json.Serialization;

namespace StrixMusic.Cores.OneDrive.Services
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
