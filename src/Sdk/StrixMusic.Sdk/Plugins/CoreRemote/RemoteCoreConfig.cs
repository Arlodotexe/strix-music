using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.AbstractUI.Models;
using OwlCore.Provisos;
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
            AbstractUIElements = new List<AbstractUICollection>();
        }

        /// <inheritdoc />
        public ICore SourceCore { get; }

        /// <inheritdoc/>
        public IServiceProvider? Services { get; private set; }

        /// <inheritdoc/>
        public IReadOnlyList<AbstractUICollection> AbstractUIElements { get; private set; }

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