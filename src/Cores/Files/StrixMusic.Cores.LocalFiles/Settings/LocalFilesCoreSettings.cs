using System.Threading.Tasks;
using System.Threading;
using OwlCore.AbstractStorage;
using OwlCore.Services;
using StrixMusic.Sdk.Services;
using System.Text.Json;

namespace StrixMusic.Cores.LocalFiles.Settings
{
    /// <summary>
    /// A container for <see cref="LocalFilesCore"/> settings.
    /// </summary>
    public sealed class LocalFilesCoreSettings : SettingsBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="LocalFilesCoreSettings"/>.
        /// </summary>
        public LocalFilesCoreSettings(IFolderData folder)
            : base(folder, FilesCoreSettingsSerializer.Singleton)
        {
        }

        /// <summary>
        /// The folder that the user has chosen to scan for this core instance.
        /// </summary>
        public string FolderPath
        {
            get => GetSetting(() => string.Empty);
            set => SetSetting(value);
        }

        /// <summary>
        /// If true, the app will not initialize the metadata repos and previously scanned data will not be loaded.
        /// </summary>
        public bool InitWithEmptyMetadataRepos
        {
            get => GetSetting(() => false);
            set => SetSetting(value);
        }

        /// <summary>
        /// If the core should use TagLib to scan file metadata.
        /// </summary>
        public bool ScanWithTagLib
        {
            get => GetSetting(() => true);
            set => SetSetting(value);
        }

        /// <summary>
        /// If the core should use file properties to scan metadata. 
        /// </summary>
        public bool ScanWithFileProperties
        {
            get => GetSetting(() => false);
            set => SetSetting(value);
        }

        /// <inheritdoc/>
        public override Task LoadAsync(CancellationToken? cancellationToken = null)
        {
            try
            {
                return base.LoadAsync(cancellationToken);
            }
            catch (JsonException)
            {
            }

            return Task.CompletedTask;
        }
    }
}
