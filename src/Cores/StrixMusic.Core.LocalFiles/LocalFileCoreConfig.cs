using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.AbstractStorage;
using OwlCore.AbstractUI.Models;
using OwlCore.Extensions;
using StrixMusic.Core.LocalFiles.Backing.Models;
using StrixMusic.Core.LocalFiles.Backing.Services;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.MediaPlayback;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrixMusic.Core.LocalFiles
{
    ///  <inheritdoc/>
    public class LocalFileCoreConfig : ICoreConfig
    {
        private IFileSystemService? _fileSystemService;
        private TrackService _trackService;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalFileCoreConfig"/> class.
        /// </summary>
        public LocalFileCoreConfig(ICore sourceCore)
        {
            SourceCore = sourceCore;
        }

        /// <inheritdoc />
        public ICore SourceCore { get; }

        /// <inheritdoc/>
        public IServiceProvider? Services { get; private set; }

        /// <inheritdoc/>
        public IReadOnlyList<AbstractUIElementGroup> AbstractUIElements { get; private set; }

        /// <inheritdoc/>
        public Uri LogoSvgUrl => throw new NotImplementedException();

        /// <inheritdoc />
        public MediaPlayerType PreferredPlayerType => MediaPlayerType.None;

        /// <summary>
        /// Configures services for this instance of the core.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task ConfigureServices(IServiceCollection services)
        {
            Services = null;

            Guard.IsNotNull(_fileSystemService, nameof(_fileSystemService));
            _trackService = new TrackService(_fileSystemService);
            services.Add(new ServiceDescriptor(typeof(TrackService), _trackService));
            Services = services.BuildServiceProvider();
        }

        /// <summary>
        /// Configures the minimum required services for core configuration in a safe manner and is guaranteed not to throw.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task SetupConfigurationServices(IServiceCollection services)
        {
            var provider = services.BuildServiceProvider();
            _fileSystemService = provider.GetRequiredService<IFileSystemService>();

            return _fileSystemService.InitAsync();
        }

        /// <summary>
        /// This method picks the folder for file core.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task SetupFileCoreFolder()
        {
            if (_fileSystemService == null)
                return;

            var folders = await _fileSystemService.GetPickedFolders();
            if (folders.Count == 0)
                await _fileSystemService.PickFolder();
        }
    }
}