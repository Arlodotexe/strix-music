using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.AbstractStorage;
using OwlCore.AbstractUI.Models;
using StrixMusic.Core.LocalFiles.Services;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Services.FileMetadataManager;
using StrixMusic.Sdk.Services.Settings;

namespace StrixMusic.Core.LocalFiles
{
    ///  <inheritdoc/>
    public class LocalFilesCoreConfig : ICoreConfig
    {
        private IFileSystemService? _fileSystemService;
        private ISettingsService? _settingsService;
        private bool _baseServicesSetup;
        private FileMetadataManager? _fileMetadataManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalFilesCoreConfig"/> class.
        /// </summary>
        public LocalFilesCoreConfig(ICore sourceCore)
        {
            SourceCore = sourceCore;
            AbstractUIElements = new List<AbstractUIElementGroup>();
        }

        /// <inheritdoc />
        public ICore SourceCore { get; }

        /// <inheritdoc/>
        public IServiceProvider? Services { get; private set; }

        /// <inheritdoc/>
        public IReadOnlyList<AbstractUIElementGroup> AbstractUIElements { get; private set; }

        /// <inheritdoc />
        public MediaPlayerType PlaybackType => MediaPlayerType.Standard;

        /// <inheritdoc/>
        public event EventHandler? AbstractUIElementsChanged;

        /// <summary>
        /// Configures services for this instance of the core.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task SetupServices(IServiceCollection services)
        {
            await SetupConfigurationServices(services);
            Guard.IsNotNull(_fileSystemService, nameof(_fileSystemService));

            var folderData = await GetConfiguredFolder();

            Guard.IsNotNull(folderData, nameof(folderData));

            _fileMetadataManager = new FileMetadataManager(SourceCore.InstanceId, folderData);
            await _fileMetadataManager.InitAsync();

            services.AddSingleton<IFileMetadataManager>(_fileMetadataManager);

            Services = null;
            Services = services.BuildServiceProvider();
        }

        /// <summary>
        /// Configures the minimum required services for core configuration in a safe manner.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task SetupConfigurationServices(IServiceCollection services)
        {
            if (_baseServicesSetup)
                return Task.CompletedTask;

            _baseServicesSetup = true;

            var fileSystemService = services.First(x => x.ServiceType == typeof(IFileSystemService)).ImplementationInstance as IFileSystemService;

            Guard.IsNotNull(fileSystemService, nameof(fileSystemService));

            _fileSystemService = fileSystemService;
            _settingsService = new LocalFilesCoreSettingsService(SourceCore.InstanceId);

            services.AddSingleton(_settingsService);

            Services = services.BuildServiceProvider();

            return _fileSystemService.InitAsync();
        }

        /// <summary>
        /// Scans metadata for the configured folders.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task ScanFileMetadata()
        {
            Guard.IsNotNull(Services, nameof(Services));
            Guard.IsNotNull(_fileMetadataManager, nameof(_fileMetadataManager));

            var folderData = await GetConfiguredFolder();

            Guard.IsNotNull(folderData, nameof(folderData));

            await _fileMetadataManager.StartScan();
        }

        /// <summary>
        /// This folder picked for this instance of the <see cref="LocalFilesCore"/>.
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

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            if (_fileMetadataManager != null)
                await _fileMetadataManager.DisposeAsync();
        }
    }
}