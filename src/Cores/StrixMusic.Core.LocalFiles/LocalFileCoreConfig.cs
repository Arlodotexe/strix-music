using Microsoft.Extensions.DependencyInjection;
using OwlCore.AbstractStorage;
using OwlCore.AbstractUI.Models;
using OwlCore.Extensions;
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
            if (_fileSystemService != null)
            {
                var folders = await _fileSystemService.GetPickedFolders();

                IFolderData folderData = null;
                if (folders.Count == 0)
                {
                    folderData = await _fileSystemService.PickFolder();
                }
                else
                {
                    folderData = folders.ToOrAsList().FirstOrDefault();
                }

                if (folderData != null)
                {
                    var files = await folderData.GetFilesAsync();
                }
            }
        }
    }
}