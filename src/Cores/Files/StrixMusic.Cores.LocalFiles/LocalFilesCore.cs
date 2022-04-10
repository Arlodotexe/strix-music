using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using OwlCore;
using OwlCore.AbstractStorage;
using OwlCore.AbstractUI.Models;
using OwlCore.Extensions;
using StrixMusic.Cores.Files;
using StrixMusic.Cores.Files.Models;
using StrixMusic.Cores.LocalFiles.Services;
using StrixMusic.Sdk.FileMetadata;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Models;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Services;

namespace StrixMusic.Cores.LocalFiles
{
    /// <inheritdoc />
    public sealed class LocalFilesCore : FilesCore
    {
        private Notification? _scannerRequiredNotification;
        private readonly LocalFilesCoreConfigPanel _configPanel = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="FilesCore"/> class.
        /// </summary>
        /// <param name="instanceId">A unique identifier for this core instance.</param>
        /// <param name="fileSystem">An abstraction of the local file system.</param>
        /// <param name="settingsStorage">A folder abstraction where this core can persist settings data beyond the lifetime of the application.</param>
        /// <param name="notificationService">A service that can notify the user with interactive UI or messages.</param>
        public LocalFilesCore(string instanceId, IFileSystemService fileSystem, IFolderData settingsStorage, INotificationService notificationService)
            : base(instanceId)
        {
            FileSystem = fileSystem;
            NotificationService = notificationService;
            Settings = new LocalFilesCoreSettings(settingsStorage);

            AttachEvents();
        }

        private void AttachEvents()
        {
            Settings.PropertyChanged += OnSettingChanged;
            _configPanel.RescanButton.Clicked += RescanButton_Clicked;
            _configPanel.UseFilePropsScannerToggle.StateChanged += UseFilePropsScannerToggle_StateChanged;
            _configPanel.UseTagLibScannerToggle.StateChanged += UseTagLibScannerToggle_StateChanged;
        }

        private void DetachEvents()
        {
            Settings.PropertyChanged -= OnSettingChanged;
            _configPanel.RescanButton.Clicked -= RescanButton_Clicked;
            _configPanel.UseFilePropsScannerToggle.StateChanged -= UseFilePropsScannerToggle_StateChanged;
            _configPanel.UseTagLibScannerToggle.StateChanged -= UseTagLibScannerToggle_StateChanged;
        }

        /// <inheritdoc/>
        public override CoreMetadata Registration { get; } = Metadata;

        /// <inheritdoc />
        public override AbstractUICollection AbstractConfigPanel => _configPanel;

        /// <inheritdoc />
        public override MediaPlayerType PlaybackType { get; } = MediaPlayerType.Standard;

        /// <summary>
        /// The metadata that identifies this core before instantiation.
        /// </summary>
        public static CoreMetadata Metadata { get; } = new CoreMetadata(id: nameof(LocalFilesCore),
                                                                        displayName: "Local Files",
                                                                        logoUri: new Uri("ms-appx:///Assets/Cores/LocalFiles/Logo.svg"),
                                                                        sdkVer: Version.Parse("0.0.0.0"));

        /// <summary>
        /// The settings for this core instance.
        /// </summary>
        internal LocalFilesCoreSettings Settings { get; }

        /// <summary>
        /// An abstraction of the local file system.
        /// </summary>
        internal IFileSystemService FileSystem { get; }

        /// <summary>
        /// Gets a service that can notify the user with interactive UI or generic messages.
        /// </summary>
        internal INotificationService NotificationService { get; }

        /// <summary>
        /// Change the <see cref="CoreState"/>.
        /// </summary>
        /// <param name="state">The new state.</param>
        public void ChangeCoreState(CoreState state)
        {
            CoreState = state;
            CoreStateChanged?.Invoke(this, state);
        }

        /// <inheritdoc/>
        public override ValueTask DisposeAsync()
        {
            // Dispose any resources not known to the SDK.
            // Do not dispose Library, Devices, etc. manually. The SDK will dispose these for you.
            FileMetadataManager?.DisposeAsync();

            _configPanel.ConfigDoneButton.Clicked -= ConfigDoneButtonOnClicked;

            DetachEvents();

            return default;
        }

        /// <inheritdoc />
        public override event EventHandler<CoreState>? CoreStateChanged;

        /// <inheritdoc />
        public override event EventHandler? AbstractConfigPanelChanged;

        /// <inheritdoc />
        public override event EventHandler<string>? InstanceDescriptorChanged;

        /// <inheritdoc/>
        public async override Task InitAsync(CancellationToken cancellationToken = default)
        {
            ChangeCoreState(CoreState.Loading);

            await Settings.LoadAsync(cancellationToken);
            await FileSystem.InitAsync(cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();

            var configuredFolder = string.IsNullOrWhiteSpace(Settings.FolderPath) ? null : await FileSystem.GetFolderFromPathAsync(Settings.FolderPath);
            cancellationToken.ThrowIfCancellationRequested();

            if (configuredFolder is null)
            {
                // Let the user change settings for the selected folder before first scan.
                ChangeCoreState(CoreState.NeedsConfiguration);

                var pickedFolder = await FileSystem.PickFolder();
                cancellationToken.ThrowIfCancellationRequested();

                // No folder selected.
                if (pickedFolder is null)
                {
                    ChangeCoreState(CoreState.Unloaded);
                    throw new OperationCanceledException();
                }

                Settings.FolderPath = pickedFolder.Path;
                _configPanel.Subtitle = pickedFolder.Path;

                AbstractConfigPanelChanged?.Invoke(this, EventArgs.Empty);

                await Flow.EventAsTask(x => _configPanel.ConfigDoneButton.Clicked += x, x => _configPanel.ConfigDoneButton.Clicked -= x, TimeSpan.FromMinutes(30));

                return;
            }

            ChangeCoreState(CoreState.Configured);
            ChangeCoreState(CoreState.Loading);

            _configPanel.Subtitle = configuredFolder.Path;
            AbstractConfigPanelChanged?.Invoke(this, EventArgs.Empty);

            // Load
            InstanceDescriptor = configuredFolder.Path;
            InstanceDescriptorChanged?.Invoke(this, InstanceDescriptor);

            FileMetadataManager = new FileMetadataManager(configuredFolder, FileSystem.RootFolder, NotificationService)
            {
                SkipRepoInit = Settings.InitWithEmptyMetadataRepos,
                ScanTypes = GetScanTypesFromSettings(),
            };

            await FileMetadataManager.InitAsync(cancellationToken);
            _ = FileMetadataManager.ScanAsync(cancellationToken);

            await Library.Cast<FilesCoreLibrary>().InitAsync(cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            ChangeCoreState(CoreState.Loaded);

            _configPanel.ConfigDoneButton.Clicked += ConfigDoneButtonOnClicked;

            IsInitialized = true;
        }

        private void ConfigDoneButtonOnClicked(object sender, EventArgs e)
        {
            ChangeCoreState(CoreState.Configured);
            ChangeCoreState(CoreState.Loaded);
        }

        private MetadataScanTypes GetScanTypesFromSettings()
        {
            var scanTypes = MetadataScanTypes.None;

            if (Settings.ScanWithTagLib)
                scanTypes |= MetadataScanTypes.TagLib;

            if (Settings.ScanWithFileProperties)
                scanTypes |= MetadataScanTypes.FileProperties;

            return scanTypes;
        }

        private async void OnSettingChanged(object sender, PropertyChangedEventArgs e)
        {
            var isFilePropToggle = e.PropertyName == nameof(LocalFilesCoreSettings.ScanWithFileProperties);
            var isTagLibToggle = e.PropertyName == nameof(LocalFilesCoreSettings.ScanWithTagLib);
            var isAnyScannerToggle = isFilePropToggle || isTagLibToggle;

            if (isAnyScannerToggle && !Settings.ScanWithFileProperties && !Settings.ScanWithTagLib)
            {
                _scannerRequiredNotification?.Dismiss();
                _scannerRequiredNotification = NotificationService.RaiseNotification("Whoops", "At least one metadata scanner is required.");

                if (isFilePropToggle)
                    _configPanel.UseFilePropsScannerToggle.State = true;

                if (isTagLibToggle)
                    _configPanel.UseTagLibScannerToggle.State = true;
            }
            else
            {
                _configPanel.UseFilePropsScannerToggle.State = Settings.ScanWithFileProperties;
                _configPanel.UseTagLibScannerToggle.State = Settings.ScanWithTagLib;
                _configPanel.InitWithEmptyReposToggle.State = Settings.InitWithEmptyMetadataRepos;
            }

            await Settings.SaveAsync();
        }

        private void RescanButton_Clicked(object sender, EventArgs e) => _ = FileMetadataManager?.ScanAsync();

        private void UseTagLibScannerToggle_StateChanged(object sender, bool e)
        {
            Settings.ScanWithTagLib = e;

            if (FileMetadataManager is null)
                return;

            // Enable or disable this scanner type.
            if (!FileMetadataManager.ScanTypes.HasFlag(MetadataScanTypes.TagLib) && e)
                FileMetadataManager.ScanTypes |= MetadataScanTypes.TagLib;
            else
                FileMetadataManager.ScanTypes ^= MetadataScanTypes.TagLib;
        }

        private void UseFilePropsScannerToggle_StateChanged(object sender, bool e)
        {
            Settings.ScanWithFileProperties = e;

            if (FileMetadataManager is null)
                return;

            // Enable or disable this scanner type.
            if (!FileMetadataManager.ScanTypes.HasFlag(MetadataScanTypes.FileProperties) && e)
                FileMetadataManager.ScanTypes |= MetadataScanTypes.FileProperties;
            else
                FileMetadataManager.ScanTypes ^= MetadataScanTypes.FileProperties;
        }
    }
}
