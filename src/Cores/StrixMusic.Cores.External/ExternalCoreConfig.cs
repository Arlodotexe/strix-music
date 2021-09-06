using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.AbstractUI.Models;
using OwlCore.Provisos;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Core.External
{
    ///  <inheritdoc/>
    public class ExternalCoreConfig : ICoreConfig
    {
        /// <summary>
        /// Creates a new instance of <see cref="ExternalCoreConfig"/>.
        /// </summary>
        public ExternalCoreConfig(string sourceCoreInstanceId)
        {
            SourceCore = ExternalCore.GetInstance(sourceCoreInstanceId);
            AbstractUIElements = new List<AbstractUIElementGroup>();
        }

        /// <inheritdoc />
        public ICore SourceCore { get; }

        /// <inheritdoc/>
        public IServiceProvider? Services { get; private set; }

        /// <inheritdoc/>
        public IReadOnlyList<AbstractUIElementGroup> AbstractUIElements { get; private set; }

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