// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System.Collections.Generic;
using OwlCore.ComponentModel;
using StrixMusic.Sdk.AdapterModels;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;

namespace StrixMusic.Sdk.Plugins.Model;

/// <summary>
/// An implementation of <see cref="ISearchResults"/> which delegates all member access to the <see cref="Inner"/> implementation,
/// unless the member is overridden in a derived class which changes the behavior.
/// </summary>
public class SearchResultsPluginBase : PlayableCollectionGroupPluginBase, IModelPlugin, ISearchResults, IDelegatable<ISearchResults>
{
    /// <summary>
    /// Creates a new instance of <see cref="PlayableCollectionGroupPluginBase"/>.
    /// </summary>
    /// <param name="registration">Metadata about the plugin which was provided during registration.</param>
    /// <param name="inner">The implementation which all member access is delegated to, unless the member is overridden in a derived class which changes the behavior.</param>
    /// <param name="pluginRoot">The plugin-enabled <see cref="IStrixDataRoot" /> which is responsible for creating this and all parent instances.</param>
    internal protected SearchResultsPluginBase(ModelPluginMetadata registration, ISearchResults inner, IStrixDataRoot pluginRoot)
        : base(registration, inner, pluginRoot)
    {
        Inner = inner;
    }

    /// <inheritdoc />
    public virtual bool Equals(ICoreSearchResults other) => Inner.Equals(other);

    /// <inheritdoc/>
    public new IReadOnlyList<ICoreSearchResults> Sources => ((IMerged<ICoreSearchResults>)Inner).Sources;

    /// <inheritdoc />
    public new ISearchResults Inner { get; }
}
