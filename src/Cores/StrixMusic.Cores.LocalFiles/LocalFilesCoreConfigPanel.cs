using OwlCore.AbstractUI.Models;
using StrixMusic.Cores.LocalFiles.Services;

namespace StrixMusic.Cores.LocalFiles
{
    /// <summary>
    /// The AbstractUI components used to configure the core.
    /// </summary>
    public class LocalFilesCoreConfigPanel : AbstractUICollection
    {
        private readonly AbstractUICollection _cacheSettings;

        /// <summary>
        /// Creates a new instance of <see cref="LocalFilesCoreConfigPanel"/>.
        /// </summary>
        public LocalFilesCoreConfigPanel(bool enableRescanButton = false)
            : base(id: nameof(LocalFilesCoreConfigPanel))
        {
            Title = "Local files settings";

            UseTagLibScannerToggle = new AbstractBoolean(nameof(LocalFilesCoreSettings.ScanWithTagLib), "Use TagLib")
            {
                Subtitle = "TagLib is more accurate and supports more formats, but is slower (recommended).",
            };

            UseFilePropsScannerToggle = new AbstractBoolean(nameof(LocalFilesCoreSettings.ScanWithFileProperties), "Use file properties")
            {
                Subtitle = "File properties are very fast, but provide less data.",
            };

            InitWithEmptyReposToggle = new AbstractBoolean(nameof(LocalFilesCoreSettings.InitWithEmptyMetadataRepos), "Ignore scan cache")
            {
                Subtitle = "Always rescan metadata on startup, ignoring data from previous scans. Requires an app restart",
            };

            RescanButton = new AbstractButton("rescan", "Scan metadata", "\uE149")
            {
                Subtitle = "Force a manual rescan of file metadata in the selected folder.",
            };

            ConfigDoneButton = new AbstractButton("FilesCoreDoneButton", "Done", null, AbstractButtonType.Confirm);

            _cacheSettings = new AbstractUICollection("cacheSettings")
            {
                Title = "Cache settings",
                Subtitle = "Requires rescan or restart",
            };

            _cacheSettings.Add(InitWithEmptyReposToggle);

            if (enableRescanButton)
                _cacheSettings.Add(RescanButton);

            var metadataScanType = new AbstractUICollection("metadataScanType")
            {
                UseTagLibScannerToggle,
                UseFilePropsScannerToggle,
            };

            metadataScanType.Title = "Scanner type";
            metadataScanType.Subtitle = "Requires restart.";

            Add(metadataScanType);
            Add(_cacheSettings);
            Add(ConfigDoneButton);
        }

        /// <summary>
        /// A toggle that indicates if the file metadata scanner should ignore and previously scanned data.
        /// </summary>
        public AbstractBoolean InitWithEmptyReposToggle { get; }

        /// <summary>
        /// A toggle that indicates if TagLibSharp should be used to read metadata from audio files.
        /// </summary>
        public AbstractBoolean UseTagLibScannerToggle { get; }

        /// <summary>
        /// A toggle that indicates if file properties should be used to read metadata from audio files.
        /// </summary>
        public AbstractBoolean UseFilePropsScannerToggle { get; }

        /// <summary>
        /// A button that, when clicked, signals that the user is finished interacting with this panel.
        /// </summary>
        public AbstractButton ConfigDoneButton { get; }

        /// <summary>
        /// A button that, when clicked, should trigger a rescan of metadata.
        /// </summary>
        public AbstractButton RescanButton { get; }

        /// <summary>
        /// Enables or disables the <see cref="RescanButton"/>.
        /// </summary>
        public void ToggleRescanButton(bool isEnabled)
        {
            if (isEnabled && !_cacheSettings.Contains(RescanButton))
                _cacheSettings.Add(RescanButton);

            if (!isEnabled && _cacheSettings.Contains(RescanButton))
                _cacheSettings.Remove(RescanButton);
        }
    }
}
