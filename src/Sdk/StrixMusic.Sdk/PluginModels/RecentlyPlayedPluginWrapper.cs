// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System.Collections.Generic;
using StrixMusic.Sdk.AdapterModels;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Sdk.Plugins.Model;

namespace StrixMusic.Sdk.PluginModels;

/// <summary>
/// Wraps an instance of <see cref="IRecentlyPlayed"/> with the provided plugins.
/// </summary>
public class RecentlyPlayedPluginWrapper : PlayableCollectionGroupPluginWrapperBase, IRecentlyPlayed, IPluginWrapper
{
    private readonly IRecentlyPlayed _recentlyPlayed;

    /// <summary>
    /// Initializes a new instance of the <see cref="RecentlyPlayedPluginWrapper"/> class.
    /// </summary>
    /// <param name="recentlyPlayed">An existing instance to wrap around and provide plugins on top of.</param>
    /// <param name="pluginRoot">The plugin-enabled <see cref="IStrixDataRoot" /> which is responsible for creating this and all parent instances.</param>
    /// <param name="plugins">The plugins to import and apply to everything returned from this wrapper.</param>
    internal RecentlyPlayedPluginWrapper(IRecentlyPlayed recentlyPlayed, IStrixDataRoot pluginRoot, params SdkModelPlugin[] plugins)
        : base(GlobalModelPluginConnector.Create(pluginRoot, new SdkModelPlugin(PluginModelWrapperInfo.Metadata, plugins)).RecentlyPlayed.Execute(recentlyPlayed), pluginRoot, plugins)
    {
        foreach (var plugin in plugins)
            ActivePlugins.Import(plugin);

        ActivePlugins = GlobalModelPluginConnector.Create(pluginRoot, ActivePlugins);

        _recentlyPlayed = ActivePlugins.RecentlyPlayed.Execute(recentlyPlayed);
    }

    /// <inheritdoc/>
    public SdkModelPlugin ActivePlugins { get; } = new(PluginModelWrapperInfo.Metadata);

    /// <inheritdoc/>
    public IReadOnlyList<ICoreRecentlyPlayed> Sources => ((IMerged<ICoreRecentlyPlayed>)_recentlyPlayed).Sources;

    /// <inheritdoc/>
    public bool Equals(ICoreRecentlyPlayed other) => _recentlyPlayed.Equals(other);
}
