using System;
using StrixMusic.Sdk.Plugins.Model;

namespace StrixMusic.Sdk.Tests.Plugins.Models
{
    public static class SdkTestPluginMetadata
    {
        public static ModelPluginMetadata Metadata { get; } = new ModelPluginMetadata(string.Empty, nameof(SdkTestPluginMetadata), "Model plugin for unit tests", new Version(0, 0, 0, 0));
    }
}
