// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using StrixMusic.Sdk.Plugins.Model;

namespace StrixMusic.Sdk.PluginModels;

/// <summary>
/// Holds the plugin metadata for the PluginModel data layer.
/// </summary>
public static class PluginModelWrapperInfo
{
    /// <inheritdoc cref="IModelPlugin.Metadata"/>
    public static ModelPluginMetadata Metadata { get; } = new("PluginModelWrapper", "Plugin model wrapper", "A data layer which wraps all returned models with a ModelPlugin.", typeof(PluginModelWrapperInfo).Assembly.GetName().Version ?? throw new System.Exception("Could not location version of current assembly"));
}
