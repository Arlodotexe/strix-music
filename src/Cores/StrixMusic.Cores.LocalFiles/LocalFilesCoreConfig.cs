using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.AbstractStorage;
using OwlCore.AbstractUI.Models;
using OwlCore.Extensions;
using StrixMusic.Cores.Files;
using StrixMusic.Cores.LocalFiles.Services;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Services.FileMetadataManager;
using StrixMusic.Sdk.Services.Notifications;
using StrixMusic.Sdk.Services.Settings;

namespace StrixMusic.Cores.LocalFiles
{
    ///  <inheritdoc/>
    public sealed class LocalFilesCoreConfig : ICoreConfig
    {
        private readonly AbstractBoolean _initWithEmptyReposToggle;
        private readonly AbstractBoolean _useTagLibScannerToggle;
        private readonly AbstractBoolean _useFilePropsScannerToggle;
        private readonly AbstractButton _configDoneButton;
        private readonly AbstractButton _rescanButton;

        private IFileSystemService? _fileSystemService;
        private ISettingsService? _settingsService;
        private FileMetadataManager? _fileMetadataManager;
        private Notification? _scannerRequiredNotification;
        private INotificationService? _notificationService;

        private bool _baseServicesSetup;
        private bool _allServicesSetup;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalFilesCoreConfig"/> class.
        /// </summary>
        public LocalFilesCoreConfig(ICore sourceCore)
        {
            SourceCore = sourceCore;

            _useTagLibScannerToggle = new AbstractBoolean(nameof(LocalFilesCoreSettingsKeys.ScanWithTagLib), "Use TagLib")
            {
                Subtitle = "TagLib is more accurate and supports more formats, but is slower (recommended).",
            };

            _useFilePropsScannerToggle = new AbstractBoolean(nameof(LocalFilesCoreSettingsKeys.ScanWithFileProperties), "Use file properties")
            {
                Subtitle = "File properties are very fast, but provide less data.",
            };

            _initWithEmptyReposToggle = new AbstractBoolean(nameof(LocalFilesCoreSettingsKeys.InitWithEmptyMetadataRepos), "Ignore scan cache")
            {
                Subtitle = "Always rescan metadata on startup, ignoring data from previous scans. Requires an app restart",
            };

            _rescanButton = new AbstractButton("rescan", "Scan metadata", "\uE149")
            {
                Subtitle = "Force a manual rescan of file metadata in the selected folder.",
            };

            _configDoneButton = new AbstractButton("FilesCoreDoneButton", "Done", null, AbstractButtonType.Confirm);

            AbstractUIElements = CreateGenericConfig().IntoList();
        }

        private void AttachEvents()
        {
            Guard.IsNotNull(_settingsService, nameof(_settingsService));

            _settingsService.SettingChanged += SettingsServiceOnSettingChanged;
            _useFilePropsScannerToggle.StateChanged += UseFilePropsScannerToggle_StateChanged;
            _useTagLibScannerToggle.StateChanged += UseTagLibScannerToggle_StateChanged;
            _initWithEmptyReposToggle.StateChanged += InitWithEmptyReposToggleOnStateChanged;
            _rescanButton.Clicked += RescanButton_Clicked;

            _configDoneButton.Clicked += ConfigDoneButton_Clicked;
        }

        private void DetachEvents()
        {
            Guard.IsNotNull(_settingsService, nameof(_settingsService));

            _settingsService.SettingChanged -= SettingsServiceOnSettingChanged;
            _useFilePropsScannerToggle.StateChanged -= UseFilePropsScannerToggle_StateChanged;
            _useTagLibScannerToggle.StateChanged -= UseTagLibScannerToggle_StateChanged;
            _initWithEmptyReposToggle.StateChanged -= InitWithEmptyReposToggleOnStateChanged;
            _rescanButton.Clicked -= RescanButton_Clicked;

            _configDoneButton.Clicked -= ConfigDoneButton_Clicked;
        }

        /// <inheritdoc />
        public ICore SourceCore { get; }

        /// <inheritdoc/>
        public IServiceProvider? Services { get; private set; }

        /// <inheritdoc/>
        public IReadOnlyList<AbstractUICollection> AbstractUIElements { get; private set; }

        /// <inheritdoc />
        public MediaPlayerType PlaybackType => MediaPlayerType.Standard;

        /// <inheritdoc/>
        public event EventHandler? AbstractUIElementsChanged;

        /// <summary>
        /// Configures services for this instance of the core. 
        /// This method is only called if the user has completed first time setup. 
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task SetupServices(IServiceCollection services)
        {
            if (_allServicesSetup)
                return;

            _allServicesSetup = true;

            await SetupConfigurationServices(services);
            Guard.IsNotNull(_fileSystemService, nameof(_fileSystemService));
            Guard.IsNotNull(_settingsService, nameof(_settingsService));

            var folderData = await GetConfiguredFolder();

            Guard.IsNotNull(folderData, nameof(folderData));

            var scanTypes = MetadataScanTypes.None;

            if (await _settingsService.GetValue<bool>(nameof(LocalFilesCoreSettingsKeys.ScanWithTagLib)))
                scanTypes |= MetadataScanTypes.TagLib;

            if (await _settingsService.GetValue<bool>(nameof(LocalFilesCoreSettingsKeys.ScanWithFileProperties)))
                scanTypes |= MetadataScanTypes.FileProperties;

            _fileMetadataManager = new FileMetadataManager(SourceCore.InstanceId, folderData)
            {
                SkipRepoInit = _initWithEmptyReposToggle.State,
                ScanTypes = scanTypes,
            };

            await _fileMetadataManager.InitAsync();
            Task.Run(_fileMetadataManager.StartScan).Forget();

            services.AddSingleton<IFileMetadataManager>(_fileMetadataManager);

            Services = null;
            Services = services.BuildServiceProvider();

            var genericConfig = CreateGenericConfig();
            genericConfig.Subtitle = folderData.Path;

            AbstractUIElements = genericConfig.IntoList();
            AbstractUIElementsChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Configures the minimum required services for core configuration in a safe manner.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task SetupConfigurationServices(IServiceCollection services)
        {
            if (_baseServicesSetup)
                return;

            _baseServicesSetup = true;

            _fileSystemService = services.First(x => x.ImplementationInstance is IFileSystemService).ImplementationInstance?.Cast<IFileSystemService>();
            _notificationService = services.First(x => x.ImplementationInstance is INotificationService).ImplementationInstance?.Cast<INotificationService>();
            _settingsService = new LocalFilesCoreSettingsService(SourceCore.InstanceId);

            services.AddSingleton(_settingsService);

            Services = services.BuildServiceProvider();

            _initWithEmptyReposToggle.State = await _settingsService.GetValue<bool>(nameof(LocalFilesCoreSettingsKeys.InitWithEmptyMetadataRepos));
            _useFilePropsScannerToggle.State = await _settingsService.GetValue<bool>(nameof(LocalFilesCoreSettingsKeys.ScanWithFileProperties));
            _useTagLibScannerToggle.State = await _settingsService.GetValue<bool>(nameof(LocalFilesCoreSettingsKeys.ScanWithTagLib));

            Guard.IsNotNull(_fileSystemService, nameof(_fileSystemService));

            await _fileSystemService.InitAsync();
            AttachEvents();
        }

        public AbstractUICollection CreateGenericConfig()
        {
            var cacheSettings = new AbstractUICollection("cacheSettings")
            {
                _initWithEmptyReposToggle,
            };

            cacheSettings.Title = "Cache settings";
            cacheSettings.Subtitle = "Requires restart.";

            if (_allServicesSetup)
                cacheSettings.Add(_rescanButton);

            var metadataScanType = new AbstractUICollection("metadataScanType")
            {
                _useTagLibScannerToggle,
                _useFilePropsScannerToggle,
            };

            metadataScanType.Title = "Scanner type";
            metadataScanType.Subtitle = "Requires restart.";

            return new AbstractUICollection("GenericConfig")
            {
                Title = "Local files settings",
                Items = new List<AbstractUIElement>
                {
                    metadataScanType,
                    cacheSettings,
                    _configDoneButton,
                },
            };
        }

        /// <summary>
        /// This folder picked for this instance of the <see cref="FilesCore"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<IFolderData?> GetConfiguredFolder()
        {
            Guard.IsNotNull(_fileSystemService, nameof(_fileSystemService));
            Guard.IsNotNull(_settingsService, nameof(_settingsService));

            var configuredPath = await _settingsService.GetValue<string>(nameof(LocalFilesCoreSettingsKeys.FolderPath));

            if (string.IsNullOrWhiteSpace(configuredPath))
                return null;

            var folderData = await _fileSystemService.GetFolderFromPathAsync(configuredPath);

            return folderData;
        }

        public void SaveAbstractUI(AbstractUICollection collection)
        {
            AbstractUIElements = collection.IntoList();
            AbstractUIElementsChanged?.Invoke(this, EventArgs.Empty);
        }

        private void ConfigDoneButton_Clicked(object sender, EventArgs e)
        {
            if (!_allServicesSetup)
                return;

            SourceCore.Cast<LocalFilesCore>().ChangeCoreState(CoreState.Configured);
            SourceCore.Cast<LocalFilesCore>().ChangeCoreState(CoreState.Loaded);
        }

        private async void InitWithEmptyReposToggleOnStateChanged(object sender, bool e)
        {
            Guard.IsNotNull(_settingsService, nameof(_settingsService));
            await _settingsService.SetValue<bool>(nameof(LocalFilesCoreSettingsKeys.InitWithEmptyMetadataRepos), e);
        }

        private async void UseTagLibScannerToggle_StateChanged(object sender, bool e)
        {
            Guard.IsNotNull(_settingsService, nameof(_settingsService));
            await _settingsService.SetValue<bool>(nameof(LocalFilesCoreSettingsKeys.ScanWithTagLib), e);
        }

        private async void UseFilePropsScannerToggle_StateChanged(object sender, bool e)
        {
            Guard.IsNotNull(_settingsService, nameof(_settingsService));
            await _settingsService.SetValue<bool>(nameof(LocalFilesCoreSettingsKeys.ScanWithFileProperties), e);
        }

        private async void SettingsServiceOnSettingChanged(object sender, SettingChangedEventArgs e)
        {
            Guard.IsNotNull(_settingsService, nameof(_settingsService));
            Guard.IsNotNull(_notificationService, nameof(_notificationService));

            if (!(e.Key == nameof(LocalFilesCoreSettingsKeys.ScanWithFileProperties) ||
                  e.Key == nameof(LocalFilesCoreSettingsKeys.ScanWithTagLib)))
                return;

            var filePropSetting = await _settingsService.GetValue<bool>(nameof(LocalFilesCoreSettingsKeys.ScanWithFileProperties));
            var tagLibSetting = await _settingsService.GetValue<bool>(nameof(LocalFilesCoreSettingsKeys.ScanWithTagLib));

            if (!filePropSetting && !tagLibSetting)
            {
                _scannerRequiredNotification?.Dismiss();
                _scannerRequiredNotification = _notificationService.RaiseNotification("Whoops", "At least one metadata scanner is required.");

                if (e.Key == nameof(LocalFilesCoreSettingsKeys.ScanWithFileProperties))
                    _useFilePropsScannerToggle.State = true;

                if (e.Key == nameof(LocalFilesCoreSettingsKeys.ScanWithTagLib))
                    _useTagLibScannerToggle.State = true;
            }
        }

        private void RescanButton_Clicked(object sender, EventArgs e)
        {
            Guard.IsNotNull(_fileMetadataManager, nameof(_fileMetadataManager));

            Task.Run(_fileMetadataManager.StartScan).Forget();
        }

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            if (_fileMetadataManager != null)
                await _fileMetadataManager.DisposeAsync();

            DetachEvents();
        }
    }
}