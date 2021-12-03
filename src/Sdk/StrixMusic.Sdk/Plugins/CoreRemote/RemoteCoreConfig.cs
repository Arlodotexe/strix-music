using System;
using System.Threading.Tasks;
using OwlCore.AbstractUI.Models;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Sdk.Plugins.CoreRemote
{
    ///  <inheritdoc/>
    public class RemoteCoreConfig : ICoreConfig
    {
        /// <summary>
        /// Creates a new instance of <see cref="RemoteCoreConfig"/>.
        /// </summary>
        public RemoteCoreConfig(string sourceCoreInstanceId)
        {
            SourceCore = RemoteCore.GetInstance(sourceCoreInstanceId);
            AbstractUIElements = new AbstractUICollection(string.Empty);
        }

        /// <inheritdoc />
        public ICore SourceCore { get; }

        /// <inheritdoc/>
        public IServiceProvider? Services { get; private set; }

        /// <inheritdoc/>
        public AbstractUICollection AbstractUIElements { get; private set; }

        /// <inheritdoc />
        public MediaPlayerType PlaybackType { get; set; }

        /// <inheritdoc/>
        public event EventHandler? AbstractUIElementsChanged;

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            return default;
        }
    }
}