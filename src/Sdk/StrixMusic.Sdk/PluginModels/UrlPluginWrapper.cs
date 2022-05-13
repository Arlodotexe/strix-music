// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StrixMusic.Sdk.AdapterModels;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Sdk.Plugins.Model;

namespace StrixMusic.Sdk.PluginModels;

/// <summary>
/// Wraps an instance of <see cref="IUrl"/> with the provided plugins.
/// </summary>
public class UrlPluginWrapper : IUrl, IPluginWrapper
{
    private readonly IUrl _url;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlayableCollectionGroupPluginWrapperBase"/> class.
    /// </summary>
    /// <param name="url">The instance to wrap around and apply plugins to.</param>
    /// <param name="plugins">The plugins that are applied to items returned from or emitted by this collection.</param>
    internal UrlPluginWrapper(IUrl url, params SdkModelPlugin[] plugins)
    {
        foreach (var item in plugins)
            ActivePlugins.Import(item);

        ActivePlugins = GlobalModelPluginConnector.Create(ActivePlugins);
        
        _url = ActivePlugins.Url.Execute(url);
        AttachEvents(_url);
    }

    private void AttachEvents(IUrl url)
    {
        url.SourcesChanged += OnSourcesChanged;
    }

    private void DetachEvents(IUrl url)
    {
        url.SourcesChanged -= OnSourcesChanged;
    }

    private void OnSourcesChanged(object sender, EventArgs e) => SourcesChanged?.Invoke(sender, e);

    /// <inheritdoc/>
    public SdkModelPlugin ActivePlugins { get; } = new(PluginModelWrapperInfo.Metadata);

    /// <inheritdoc/>
    public ValueTask DisposeAsync()
    {
        DetachEvents(_url);
        return _url.DisposeAsync();
    }

    /// <inheritdoc/>
    public bool Equals(ICoreUrl other) => _url.Equals(other);

    /// <inheritdoc/>
    public IReadOnlyList<ICoreUrl> Sources => _url.Sources;
        
    /// <inheritdoc cref="IMerged.SourcesChanged"/>
    public event EventHandler? SourcesChanged;

    /// <inheritdoc/>
    public string Label => _url.Label;

    /// <inheritdoc/>
    public Uri Url => _url.Url;

    /// <inheritdoc/>
    public UrlType Type => _url.Type;
}
