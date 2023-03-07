// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;

namespace StrixMusic.Sdk.Plugins.Model
{
    /// <summary>
    /// Contains metadata for a plugin. Used to identify a plugin before instantiation.
    /// </summary>
    public sealed class ModelPluginMetadata
    {
        /// <summary>
        /// Creates a new instance of <see cref="ModelPluginMetadata"/>.
        /// </summary>
        /// <param name="id">A unique identifier for this plugin, including all instances.</param>
        /// <param name="displayName">The user-friendly name of the plugin.</param>
        /// <param name="description">Briefly describes this plugin.</param>
        /// <param name="pluginVersion">This plugin's version number.</param>
        public ModelPluginMetadata(string id, string displayName, string description, Version pluginVersion)
        {
            Id = id;
            DisplayName = displayName;
            Description = description;
            SdkVersion = typeof(ModelPluginMetadata).Assembly.GetName().Version ?? throw new Exception("Could not location current assembly version.");
            PluginVersion = pluginVersion;
        }

        /// <summary>
        /// A unique identifier for this plugin, across all instances.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// A path pointing to an SVG file containing the logo for this plugin.
        /// </summary>
        public Uri? LogoUri { get; set; }

        /// <summary>
        /// The user-friendly name for the plugin.
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Briefly describes this plugin.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// The version of the Strix Music SDK that this plugin was built against.
        /// </summary>
        public Version SdkVersion { get; }

        /// <summary>
        /// This plugin's version number.
        /// </summary>
        public Version PluginVersion { get; set; }
    }
}
