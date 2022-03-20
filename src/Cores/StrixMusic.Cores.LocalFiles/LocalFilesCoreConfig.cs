using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.AbstractStorage;
using OwlCore.AbstractUI.Models;
using OwlCore.Extensions;
using StrixMusic.Cores.Files;
using StrixMusic.Cores.LocalFiles.Services;
using StrixMusic.Sdk.Models;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Services;
using StrixMusic.Sdk.FileMetadata;

namespace StrixMusic.Cores.LocalFiles
{
    ///  <inheritdoc/>
    public sealed class LocalFilesCoreConfig : ICoreConfig
    {
        private readonly LocalFilesCore _sourceCore;
        private readonly LocalFilesCoreSettings _settings;

        private readonly AbstractBoolean _initWithEmptyReposToggle;
        private readonly AbstractBoolean _useTagLibScannerToggle;
        private readonly AbstractBoolean _useFilePropsScannerToggle;
        private readonly AbstractButton _configDoneButton;
        private readonly AbstractButton _rescanButton;

        private IFileSystemService _fileSystem;
        private INotificationService _notificationService;
        private Notification? _scannerRequiredNotification;

        private bool _baseServicesSetup;
        private bool _allServicesSetup;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalFilesCoreConfig"/> class.
        /// </summary>
        public LocalFilesCoreConfig(LocalFilesCore sourceCore)
        {
            _sourceCore = sourceCore;
            _settings = sourceCore.Settings;
            _notificationService = sourceCore.NotificationService;
            _fileSystem = sourceCore.FileSystem;

            _useTagLibScannerToggle = new AbstractBoolean(nameof(LocalFilesCoreSettings.ScanWithTagLib), "Use TagLib")
            {
                Subtitle = "TagLib is more accurate and supports more formats, but is slower (recommended).",
            };

            _useFilePropsScannerToggle = new AbstractBoolean(nameof(LocalFilesCoreSettings.ScanWithFileProperties), "Use file properties")
            {
                Subtitle = "File properties are very fast, but provide less data.",
            };

            _initWithEmptyReposToggle = new AbstractBoolean(nameof(LocalFilesCoreSettings.InitWithEmptyMetadataRepos), "Ignore scan cache")
            {
                Subtitle = "Always rescan metadata on startup, ignoring data from previous scans. Requires an app restart",
            };

            _rescanButton = new AbstractButton("rescan", "Scan metadata", "\uE149")
            {
                Subtitle = "Force a manual rescan of file metadata in the selected folder.",
            };

            _configDoneButton = new AbstractButton("FilesCoreDoneButton", "Done", null, AbstractButtonType.Confirm);

            AbstractUIElements = CreateGenericConfig();
        }

        private void AttachEvents()
        {
            _sourceCore.Settings.PropertyChanged += OnSettingChanged;
            _useFilePropsScannerToggle.StateChanged += UseFilePropsScannerToggle_StateChanged;
            _useTagLibScannerToggle.StateChanged += UseTagLibScannerToggle_StateChanged;
            _initWithEmptyReposToggle.StateChanged += InitWithEmptyReposToggleOnStateChanged;
            _rescanButton.Clicked += RescanButton_Clicked;

            _configDoneButton.Clicked += ConfigDoneButton_Clicked;
        }

        private void DetachEvents()
        {
            _sourceCore.Settings.PropertyChanged -= OnSettingChanged;
            _useFilePropsScannerToggle.StateChanged -= UseFilePropsScannerToggle_StateChanged;
            _useTagLibScannerToggle.StateChanged -= UseTagLibScannerToggle_StateChanged;
            _initWithEmptyReposToggle.StateChanged -= InitWithEmptyReposToggleOnStateChanged;
            _rescanButton.Clicked -= RescanButton_Clicked;

            _configDoneButton.Clicked -= ConfigDoneButton_Clicked;
        }

        /// <inheritdoc />
        public ICore SourceCore => _sourceCore;

        /// <inheritdoc/>
        public AbstractUICollection AbstractUIElements { get; private set; }

        /// <inheritdoc />
        public MediaPlayerType PlaybackType => MediaPlayerType.Standard;

        /// <inheritdoc/>
        public event EventHandler? AbstractUIElementsChanged;

        /// <summary>
        /// Configures services for this instance of the core. 
        /// This method is only called if the user has completed first time setup. 
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task SetupServices(IFolderData folderToScan)
        {
            if (_allServicesSetup)
                return;

            _allServicesSetup = true;

            await SetupConfigurationServices();

            var scanTypes = MetadataScanTypes.None;

            if (_settings.ScanWithTagLib)
                scanTypes |= MetadataScanTypes.TagLib;

            if (_settings.ScanWithFileProperties)
                scanTypes |= MetadataScanTypes.FileProperties;

            _sourceCore.FileMetadataManager = new FileMetadataManager(rootFolderToScan: folderToScan, metadataStorage: _fileSystem.RootFolder, _notificationService)
            {
                SkipRepoInit = _initWithEmptyReposToggle.State,
                ScanTypes = scanTypes,
            };

            await _sourceCore.FileMetadataManager.InitAsync();
            Task.Run(_sourceCore.FileMetadataManager.StartScan).Forget();

            var genericConfig = CreateGenericConfig();
            genericConfig.Subtitle = folderToScan.Path;

            AbstractUIElements = genericConfig;
            AbstractUIElementsChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Configures the minimum required services for core configuration.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task SetupConfigurationServices()
        {
            if (_baseServicesSetup)
                return;

            _baseServicesSetup = true;
            await _settings.LoadAsync();

            _initWithEmptyReposToggle.State = _settings.InitWithEmptyMetadataRepos;
            _useFilePropsScannerToggle.State = _settings.ScanWithFileProperties;
            _useTagLibScannerToggle.State = _settings.ScanWithTagLib;

            Guard.IsNotNull(_fileSystem, nameof(_fileSystem));

            await _fileSystem.InitAsync();
            AttachEvents();
        }

        public AbstractUICollection CreateGenericConfig()
        {
            var cacheSettings = new AbstractUICollection("cacheSettings")
            {
                _initWithEmptyReposToggle,
            };

            cacheSettings.Title = "Cache settings";
            cacheSettings.Subtitle = "Requires rescan or restart.";

            if (_allServicesSetup)
                cacheSettings.Add(_rescanButton);

            var metadataScanType = new AbstractUICollection("metadataScanType")
            {
                _useTagLibScannerToggle,
                _useFilePropsScannerToggle,
            };

            metadataScanType.Title = "Scanner type";
            metadataScanType.Subtitle = "Requires restart.";

            var genericConfig = new AbstractUICollection("GenericConfig")
            {
                metadataScanType,
                cacheSettings,
                _configDoneButton,
            };

            genericConfig.Title = "Local files settings";

            return genericConfig;
        }

        public void SaveAbstractUI(AbstractUICollection collection)
        {
            AbstractUIElements = collection;
            AbstractUIElementsChanged?.Invoke(this, EventArgs.Empty);
        }

        private void ConfigDoneButton_Clicked(object sender, EventArgs e)
        {
            if (!_allServicesSetup)
                return;

            SourceCore.Cast<LocalFilesCore>().ChangeCoreState(CoreState.Configured);
            SourceCore.Cast<LocalFilesCore>().ChangeCoreState(CoreState.Loaded);
        }

        private void InitWithEmptyReposToggleOnStateChanged(object sender, bool e)
        {
            Guard.IsNotNull(_settings, nameof(_settings));
            _settings.InitWithEmptyMetadataRepos = e;
        }

        private void UseTagLibScannerToggle_StateChanged(object sender, bool e)
        {
            Guard.IsNotNull(_settings, nameof(_settings));
            _settings.ScanWithTagLib = e;

            if (_sourceCore.FileMetadataManager is not null)
            {
                // Enable or disable this scanner type.
                if (!_sourceCore.FileMetadataManager.ScanTypes.HasFlag(MetadataScanTypes.TagLib) && e)
                    _sourceCore.FileMetadataManager.ScanTypes |= MetadataScanTypes.TagLib;
                else
                    _sourceCore.FileMetadataManager.ScanTypes ^= MetadataScanTypes.TagLib;
            }
        }

        private void UseFilePropsScannerToggle_StateChanged(object sender, bool e)
        {
            Guard.IsNotNull(_settings, nameof(_settings));
            _settings.ScanWithFileProperties = e;

            if (_sourceCore.FileMetadataManager is not null)
            {
                // Enable or disable this scanner type.
                if (!_sourceCore.FileMetadataManager.ScanTypes.HasFlag(MetadataScanTypes.FileProperties) && e)
                    _sourceCore.FileMetadataManager.ScanTypes |= MetadataScanTypes.FileProperties;
                else
                    _sourceCore.FileMetadataManager.ScanTypes ^= MetadataScanTypes.FileProperties;
            }
        }

        private async void OnSettingChanged(object sender, PropertyChangedEventArgs e)
        {
            Guard.IsNotNull(_settings, nameof(_settings));
            Guard.IsNotNull(_notificationService, nameof(_notificationService));

            var isFilePropToggle = e.PropertyName == nameof(LocalFilesCoreSettings.ScanWithFileProperties);
            var isTagLibToggle = e.PropertyName == nameof(LocalFilesCoreSettings.ScanWithTagLib);
            var isAnyScannerToggle = isFilePropToggle || isTagLibToggle;

            if (isAnyScannerToggle && !_settings.ScanWithFileProperties && !_settings.ScanWithTagLib)
            {
                _scannerRequiredNotification?.Dismiss();
                _scannerRequiredNotification = _notificationService.RaiseNotification("Whoops", "At least one metadata scanner is required.");

                if (isFilePropToggle)
                    _useFilePropsScannerToggle.State = true;

                if (isTagLibToggle)
                    _useTagLibScannerToggle.State = true;
            }

            await _settings.SaveAsync();
        }

        private void RescanButton_Clicked(object sender, EventArgs e)
        {
            Guard.IsNotNull(_sourceCore.FileMetadataManager, nameof(_sourceCore.FileMetadataManager));

            Task.Run(_sourceCore.FileMetadataManager.StartScan).Forget();
        }

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            if (_sourceCore.FileMetadataManager != null)
                await _sourceCore.FileMetadataManager.DisposeAsync();

            DetachEvents();
        }
    }
}
