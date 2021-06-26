using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.AbstractStorage;
using OwlCore.AbstractUI.Models;
using StrixMusic.Core.External.Services;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Services.FileMetadataManager;
using StrixMusic.Sdk.Services.Settings;

namespace StrixMusic.Core.External
{
    ///  <inheritdoc/>
    public class ExternalCoreConfig : ICoreConfig
    {
        private IFileSystemService? _fileSystemService;
        private ISettingsService? _settingsService;
        private bool _baseServicesSetup;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalCoreConfig"/> class.
        /// </summary>
        public ExternalCoreConfig(ICore sourceCore)
        {
            SourceCore = sourceCore;
            AbstractUIElements = new List<AbstractUIElementGroup>();
            SetupAbstractUISettings();
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
            Guard.IsNotNull(_settingsService, nameof(_settingsService));

            var folderData = await GetConfiguredFolder();

            Guard.IsNotNull(folderData, nameof(folderData));

            Services = null;
            Services = services.BuildServiceProvider();
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

            _settingsService = new ExternalCoreSettingsService(SourceCore.InstanceId);

            services.AddSingleton(_settingsService);

            Services = services.BuildServiceProvider();
        }

        /// <summary>
        /// This folder picked for this instance of the <see cref="ExternalCore"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<IFolderData?> GetConfiguredFolder()
        {
            Guard.IsNotNull(_fileSystemService, nameof(_fileSystemService));
            Guard.IsNotNull(_settingsService, nameof(_settingsService));

            var configuredPath = await _settingsService.GetValue<string>(nameof(ExternalCoreSettingsKeys.FolderPath));

            if (string.IsNullOrWhiteSpace(configuredPath))
                return null;

            var folderData = await _fileSystemService.GetFolderFromPathAsync(configuredPath);

            return folderData;
        }

        private void SetupAbstractUISettings()
        {
        }

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            return default;
        }
    }
}