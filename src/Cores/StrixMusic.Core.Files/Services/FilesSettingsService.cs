using StrixMusic.Core.Files;
using StrixMusic.Services.Settings;

namespace StrixMusic.Core.Files.Services
{
    /// <summary>
    /// Handles settings for the <see cref="FileCore"/>.
    /// </summary>
    public class FilesSettingsService : SettingsServiceBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilesSettingsService"/> class.
        /// </summary>
        /// <param name="instanceId"></param>
        public FilesSettingsService(string instanceId)
        {
            Id = instanceId;
        }

        /// <inheritdoc/>
        public override string Id { get; }
    }
}
