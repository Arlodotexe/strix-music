// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Threading.Tasks;
using OwlCore.AbstractUI.Models;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Models.Core;

namespace StrixMusic.Sdk.Plugins.CoreRemote
{
    /// <summary>
    /// Wraps around an instance of an <see cref="ICoreConfig"/> to enable controlling it remotely, or takes a remotingId to control another instance remotely.
    /// </summary>
    public sealed class RemoteCoreConfig : ICoreConfig
    {
        /// <summary>
        /// Creates a new instance of <see cref="RemoteCoreConfig"/>.
        /// </summary>
        internal RemoteCoreConfig(string sourceCoreInstanceId)
        {
            SourceCore = RemoteCore.GetInstance(sourceCoreInstanceId, OwlCore.Remoting.RemotingMode.Client);
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