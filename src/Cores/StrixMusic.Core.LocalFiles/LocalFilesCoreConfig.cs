using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.AbstractStorage;
using OwlCore.AbstractUI.Models;
using OwlCore.Extensions;
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
        private AbstractBoolean? _initWithEmptyReposToggle;
        private AbstractButton? _configDoneButton;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalFilesCoreConfig"/> class.
        /// </summary>
        public LocalFilesCoreConfig(ICore sourceCore)
        {
            SourceCore = sourceCore;
            AbstractUIElements = new List<AbstractUICollection>();
            SetupAbstractUISettings();
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
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task SetupServices(IServiceCollection services)
        {
            await SetupConfigurationServices(services);
            Guard.IsNotNull(_fileSystemService, nameof(_fileSystemService));
            Guard.IsNotNull(_settingsService, nameof(_settingsService));

            var folderData = await GetConfiguredFolder();

            Guard.IsNotNull(folderData, nameof(folderData));

            _fileMetadataManager = new FileMetadataManager(SourceCore.InstanceId, folderData);
            _fileMetadataManager.SkipRepoInit = await _settingsService.GetValue<bool>(nameof(LocalFilesCoreSettingsKeys.InitWithEmptyMetadataRepos));

            await _fileMetadataManager.InitAsync();
            Task.Run(_fileMetadataManager.StartScan).Forget();

            services.AddSingleton<IFileMetadataManager>(_fileMetadataManager);

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

            var fileSystemService = services.First(x => x.ServiceType == typeof(IFileSystemService)).ImplementationInstance as IFileSystemService;

            Guard.IsNotNull(fileSystemService, nameof(fileSystemService));

            _fileSystemService = fileSystemService;
            _settingsService = new LocalFilesCoreSettingsService(SourceCore.InstanceId);

            Guard.IsNotNull(_configDoneButton, nameof(_configDoneButton));
            Guard.IsNotNull(_initWithEmptyReposToggle, nameof(_initWithEmptyReposToggle));
            _initWithEmptyReposToggle.State = await _settingsService.GetValue<bool>(nameof(LocalFilesCoreSettingsKeys.InitWithEmptyMetadataRepos));

            services.AddSingleton(_settingsService);

            Services = services.BuildServiceProvider();

            await _fileSystemService.InitAsync();
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

        private void SetupAbstractUISettings()
        {
            _initWithEmptyReposToggle = new AbstractBoolean("InitWithEmptyMetadataRepos", string.Empty)
            {
                Title = "Ignore cache",
                Subtitle = "Don't use any previously scanned metadata when scanning files on startup. Requires an app restart",
            };

            _configDoneButton = new AbstractButton("LocalFilesCoreDoneButton", "Done", null, AbstractButtonType.Confirm);

            _initWithEmptyReposToggle.StateChanged += InitWithEmptyReposToggleOnStateChanged;
            _configDoneButton.Clicked += ConfigDoneButton_Clicked;

            AbstractUIElements = new List<AbstractUICollection>
            {
                new AbstractUICollection("SettingsGroup")
                {
                    _initWithEmptyReposToggle,
                    _configDoneButton,
                },
            };
        }

        private void ConfigDoneButton_Clicked(object sender, EventArgs e)
        {
            SourceCore.Cast<LocalFilesCore>().ChangeCoreState(Sdk.Data.CoreState.Configured);
            SourceCore.Cast<LocalFilesCore>().ChangeCoreState(Sdk.Data.CoreState.Loaded);
        }

        private async void InitWithEmptyReposToggleOnStateChanged(object sender, bool e)
        {
            Guard.IsNotNull(_settingsService, nameof(_settingsService));
            await _settingsService.SetValue<bool>(nameof(LocalFilesCoreSettingsKeys.InitWithEmptyMetadataRepos), e);
        }

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            if (_fileMetadataManager != null)
                await _fileMetadataManager.DisposeAsync();
        }
    }
}