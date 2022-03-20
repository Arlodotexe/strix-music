using System;
using System.Linq;
using System.Threading.Tasks;
using OwlCore;
using OwlCore.AbstractStorage;
using OwlCore.AbstractUI.Models;
using OwlCore.Extensions;
using StrixMusic.Cores.Files;
using StrixMusic.Cores.Files.Models;
using StrixMusic.Cores.LocalFiles.Services;
using StrixMusic.Sdk.Models;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Services;

namespace StrixMusic.Cores.LocalFiles
{
    /// <inheritdoc />
    public sealed class LocalFilesCore : FilesCore
    {
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
            CoreConfig = new LocalFilesCoreConfig(this);
        }

        /// <inheritdoc/>
        public override CoreMetadata Registration { get; } = Metadata;

        /// <summary>
        /// The metadata that identifies this core before instantiation.
        /// </summary>
        public static CoreMetadata Metadata { get; } = new CoreMetadata(id: nameof(LocalFilesCore),
                                                                        displayName: "Local Files",
                                                                        logoUri: new Uri("ms-appx:///Assets/Cores/LocalFiles/Logo.svg"),
                                                                        sdkVer: Version.Parse("0.0.0.0"));

        /// <inheritdoc/>
        public override ICoreConfig CoreConfig { get; protected set; }

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
            return default;
        }

        /// <inheritdoc />
        public override event EventHandler<CoreState>? CoreStateChanged;

        /// <inheritdoc />
        public override event EventHandler<string>? InstanceDescriptorChanged;

        /// <inheritdoc/>
        public async override Task InitAsync()
        {
            ChangeCoreState(CoreState.Loading);

            if (CoreConfig is not LocalFilesCoreConfig coreConfig)
                return;

            await coreConfig.SetupConfigurationServices();

            var ui = coreConfig.CreateGenericConfig();
            var confirmButton = (AbstractButton)ui.First(x => x is AbstractButton { Type: AbstractButtonType.Confirm });

            await Settings.LoadAsync();
            var configuredFolder = string.IsNullOrWhiteSpace(Settings.FolderPath) ? null : await FileSystem.GetFolderFromPathAsync(Settings.FolderPath);
            if (configuredFolder is null)
            {
                var pickedFolder = await FileSystem.PickFolder();

                // No folder selected.
                if (pickedFolder is null)
                {
                    ChangeCoreState(CoreState.Unloaded);
                    return;
                }

                Settings.FolderPath = pickedFolder.Path;
                await Settings.SaveAsync();

                ui.Subtitle = pickedFolder.Path;

                coreConfig.SaveAbstractUI(ui);

                // Let the user change settings for the selected folder before first scan.
                ChangeCoreState(CoreState.NeedsSetup);

                _ = await Flow.EventAsTask(x => confirmButton.Clicked += x, x => confirmButton.Clicked -= x, TimeSpan.FromMinutes(30));

                ChangeCoreState(CoreState.Configured);
                await InitAsync();
                return;
            }

            ui.Subtitle = configuredFolder.Path;

            coreConfig.SaveAbstractUI(ui);

            ChangeCoreState(CoreState.Configured);

            InstanceDescriptor = configuredFolder.Path;
            InstanceDescriptorChanged?.Invoke(this, InstanceDescriptor);

            await coreConfig.SetupServices(configuredFolder);
            await Library.Cast<FilesCoreLibrary>().InitAsync();

            ChangeCoreState(CoreState.Loaded);
            IsInitialized = true;
        }
    }
}
