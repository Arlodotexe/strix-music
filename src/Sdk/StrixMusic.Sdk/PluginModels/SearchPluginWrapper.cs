// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using StrixMusic.Sdk.AdapterModels;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Sdk.Plugins.Model;

namespace StrixMusic.Sdk.PluginModels;

/// <summary>
/// Wraps an instance of <see cref="ISearch"/> with the provided plugins.
/// </summary>
public class SearchPluginWrapper : ISearch, IPluginWrapper
{
    private readonly ISearch _search;

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchPluginWrapper"/> class.
    /// </summary>
    /// <param name="search">The instance to wrap around and apply plugins to.</param>
    /// <param name="plugins">The plugins that are applied to items returned from or emitted by this collection.</param>
    public SearchPluginWrapper(ISearch search, params SdkModelPlugin[] plugins)
    {
        foreach (var item in plugins)
            ActivePlugins.Import(item);

        ActivePlugins = GlobalModelPluginConnector.Create(ActivePlugins);

        _search = search;
        
        if (search.SearchHistory is not null)
            SearchHistory = new SearchHistoryPluginWrapper(search.SearchHistory, plugins);
        
        AttachEvents(_search);
    }

    private void AttachEvents(ISearch search)
    {
        search.SourcesChanged += OnSourcesChanged;
    }

    private void DetachEvents(ISearch search)
    {
        search.SourcesChanged -= OnSourcesChanged;
    }

    /// <inheritdoc/>
    public SdkModelPlugin ActivePlugins { get; } = new(PluginModelWrapperInfo.Metadata);
    
    private void OnSourcesChanged(object sender, EventArgs e) => SourcesChanged?.Invoke(sender, e);
    
    /// <inheritdoc cref="IMerged.SourcesChanged"/>
    public event EventHandler? SourcesChanged;

    /// <inheritdoc/>
    public IAsyncEnumerable<string> GetSearchAutoCompleteAsync(string query, CancellationToken cancellationToken = default) => _search.GetSearchAutoCompleteAsync(query, cancellationToken);

    /// <inheritdoc/>
    public bool Equals(ICoreSearch other) => _search.Equals(other);

    /// <inheritdoc/>
    public IReadOnlyList<ICoreSearch> Sources => _search.Sources;

    /// <inheritdoc/>
    public Task<ISearchResults> GetSearchResultsAsync(string query, CancellationToken cancellationToken = default) => _search.GetSearchResultsAsync(query, cancellationToken);

    /// <inheritdoc/>
    public IAsyncEnumerable<ISearchQuery> GetRecentSearchQueries(CancellationToken cancellationToken = default) => _search.GetRecentSearchQueries(cancellationToken);

    /// <inheritdoc/>
    public ISearchHistory? SearchHistory { get; }
}
