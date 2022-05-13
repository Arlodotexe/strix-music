// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using StrixMusic.Sdk.AdapterModels;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Sdk.Plugins.Model;

namespace StrixMusic.Sdk.PluginModels;

/// <summary>
/// Wraps an instance of <see cref="ISearchHistory"/> with the provided plugins.
/// </summary>
public class SearchHistoryPluginWrapper : PlayableCollectionGroupPluginWrapperBase, ISearchHistory, IPluginWrapper
{
    private readonly ISearchHistory _searchHistory;

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchHistoryPluginWrapper"/> class.
    /// </summary>
    /// <param name="searchHistory">An existing instance to wrap around and provide plugins on top of.</param>
    /// <param name="plugins">The plugins to import and apply to everything returned from this wrapper.</param>
    internal SearchHistoryPluginWrapper(ISearchHistory searchHistory, params SdkModelPlugin[] plugins)
        : base(GlobalModelPluginConnector.Create(plugins.Aggregate((x, y) =>
        {
            x.Import(y);
            return x;
        })).SearchHistory.Execute(searchHistory), plugins)
    {
        foreach (var plugin in plugins)
            ActivePlugins.Import(plugin);

        ActivePlugins = GlobalModelPluginConnector.Create(ActivePlugins);

        _searchHistory = ActivePlugins.SearchHistory.Execute(searchHistory);
    }

    /// <inheritdoc/>
    public SdkModelPlugin ActivePlugins { get; } = new(PluginModelWrapperInfo.Metadata);

    /// <inheritdoc/>
    public IReadOnlyList<ICoreSearchHistory> Sources => ((IMerged<ICoreSearchHistory>)_searchHistory).Sources;

    /// <inheritdoc/>
    public bool Equals(ICoreSearchHistory other) => _searchHistory.Equals(other);
}
