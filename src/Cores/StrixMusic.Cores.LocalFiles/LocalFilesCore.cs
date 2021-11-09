using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Diagnostics;
using OwlCore;
using OwlCore.AbstractStorage;
using OwlCore.AbstractUI.Models;
using OwlCore.Extensions;
using StrixMusic.Cores.Files;
using StrixMusic.Cores.Files.Models;
using StrixMusic.Cores.LocalFiles.Services;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.Services.Settings;

namespace StrixMusic.Cores.LocalFiles
{
    /// <inheritdoc />
    public sealed partial class LocalFilesCore : FilesCore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilesCore"/> class.
        /// </summary>
        /// <param name="instanceId"></param>
        public LocalFilesCore(string instanceId)
            : base(instanceId)
        {
            CoreConfig = new LocalFilesCoreConfig(this);
        }

        /// <inheritdoc/>
        public override string CoreRegistryId => nameof(LocalFilesCore);

        /// <inheritdoc/>
        public override ICoreConfig CoreConfig { get; protected set; }

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
        public override async Task InitAsync(IServiceCollection services)
        {
            Guard.IsNotNull(services, nameof(services));

            ChangeCoreState(CoreState.Loading);

            if (!(CoreConfig is LocalFilesCoreConfig coreConfig))
                return;

            await coreConfig.SetupConfigurationServices(services);

            var ui = coreConfig.CreateGenericConfig();
            var confirmButton = (AbstractButton)ui.First(x => x is AbstractButton { Type: AbstractButtonType.Confirm });

            var configuredFolder = await coreConfig.GetConfiguredFolder();
            if (configuredFolder is null)
            {
                var fileSystem = SourceCore.GetService<IFileSystemService>();
                var pickedFolder = await fileSystem.PickFolder();

                // No folder selected.
                if (pickedFolder is null)
                {
                    ChangeCoreState(CoreState.Unloaded);
                    return;
                }

                await SourceCore.GetService<ISettingsService>().SetValue<string?>(nameof(LocalFilesCoreSettingsKeys.FolderPath), pickedFolder.Path);
                ui.Subtitle = pickedFolder.Path;

                coreConfig.SaveAbstractUI(ui);

                // Let the user change settings for the selected folder before first scan.
                ChangeCoreState(CoreState.NeedsSetup);

                _ = await Flow.EventAsTask(x => confirmButton.Clicked += x, x => confirmButton.Clicked -= x, TimeSpan.FromMinutes(30));

                ChangeCoreState(CoreState.Configured);
                await InitAsync(services);
                return;
            }

            ui.Subtitle = configuredFolder.Path;

            coreConfig.SaveAbstractUI(ui);

            ChangeCoreState(CoreState.Configured);

            InstanceDescriptor = configuredFolder.Path;
            InstanceDescriptorChanged?.Invoke(this, InstanceDescriptor);

            await coreConfig.SetupServices(services);
            await Library.Cast<FilesCoreLibrary>().InitAsync();

            Guard.IsNotNull(CoreConfig.Services, nameof(CoreConfig.Services));
            ChangeCoreState(CoreState.Loaded);
        }

    }
}
