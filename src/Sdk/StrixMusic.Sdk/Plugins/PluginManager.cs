using System;
using System.Collections.Generic;
using System.Text;
using OwlCore.ComponentModel;
using StrixMusic.Sdk.Plugins.Model;

namespace StrixMusic.Sdk.Plugins
{
    /// <summary>
    /// Manages all available and configured plugins in the SDK.
    /// </summary>
    public class PluginManager
    {
        /// <summary>
        /// Manages plugins that modify the behavior of plugins or models.
        /// </summary>
        public ModelPluginManager ModelPlugins { get; } = new();
    }
}
