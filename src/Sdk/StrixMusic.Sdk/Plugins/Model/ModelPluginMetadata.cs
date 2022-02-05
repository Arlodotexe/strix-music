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
        /// <param name="sdkVersion">The version of the Strix Music SDK that this plugin was built against.</param>
        public ModelPluginMetadata(string id, string displayName, Version sdkVersion)
        {
            Id = id;
            DisplayName = displayName;
            SdkVersion = sdkVersion;
        }

        /// <summary>
        /// A unique identifier for this plugin, across all instances.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// A relative path pointing to a SVG file containing the logo for this logo.
        /// </summary>
        public Uri? LogoUri { get; set; }

        /// <summary>
        /// The user-friendly name for the plugin.
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// The version of the Strix Music SDK that this plugin was built against.
        /// </summary>
        public Version SdkVersion { get; }
    }
}