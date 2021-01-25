using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.AbstractStorage;
using OwlCore.AbstractUI.Models;
using StrixMusic.Core.LocalFiles.Backing.Models;
using StrixMusic.Core.LocalFiles.Backing.Services;
using StrixMusic.Core.LocalFiles.Extensions;
using StrixMusic.Core.LocalFiles.MetadataScanner;
using StrixMusic.Core.LocalFiles.Services;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Services.Notifications;
using StrixMusic.Sdk.Services.Settings;

namespace StrixMusic.Core.LocalFiles
{
    ///  <inheritdoc/>
    public class LocalFileCoreConfig : ICoreConfig
    {
        private IFileSystemService? _fileSystemService;
        private FileMetadataScanner? _fileMetadataScanner;
        private ISettingsService? _settingsService;
        private bool _baseServicesSetup;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalFileCoreConfig"/> class.
        /// </summary>
        public LocalFileCoreConfig(ICore sourceCore)
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

        /// <inheritdoc/>
        public Uri LogoSvgUrl => new Uri("ms-appx:///Assets/Strix/logo.svg");

        /// <inheritdoc />
        public MediaPlayerType PreferredPlayerType => MediaPlayerType.None;

        /// <summary>
        /// Configures services for this instance of the core.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task ConfigureServices(IServiceCollection services)
        {
            await SetupConfigurationServices(services);
            Guard.IsNotNull(_fileSystemService, nameof(_fileSystemService));

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

            _fileMetadataScanner = new FileMetadataScanner();

            services.Add(new ServiceDescriptor(typeof(FileMetadataScanner), new FileMetadataScanner()));
            services.Add(new ServiceDescriptor(typeof(ArtistService), new ArtistService(_fileSystemService, _fileMetadataScanner)));
            services.Add(new ServiceDescriptor(typeof(TrackService), new TrackService(_fileSystemService, _fileMetadataScanner)));
            services.Add(new ServiceDescriptor(typeof(AlbumService), new AlbumService(_fileSystemService, _fileMetadataScanner)));
            services.Add(new ServiceDescriptor(typeof(FileMetadataScanner), _fileMetadataScanner));
            services.Add(new ServiceDescriptor(typeof(PlaylistMetadata), new PlaylistService(_fileSystemService)));

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

            Guard.IsNotNull(_fileMetadataScanner, nameof(_fileMetadataScanner));

            var folderData = await GetConfiguredFolder();

            Guard.IsNotNull(folderData, nameof(folderData));

            var notificationService = Services.GetRequiredService<INotificationService>();

            var notification = notificationService.RaiseNotification("Searching for music", $"Scanning {folderData.Path}...");
            notification.Dismiss();
            await _fileMetadataScanner.ScanFolderForMetadata(folderData);
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
    }
}