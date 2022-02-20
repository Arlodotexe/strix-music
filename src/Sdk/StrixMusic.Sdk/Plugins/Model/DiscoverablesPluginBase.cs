﻿// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System.Collections.Generic;
using OwlCore.ComponentModel;
using StrixMusic.Sdk.Models;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Models.Merged;

namespace StrixMusic.Sdk.Plugins.Model;

/// <summary>
/// An implementation of <see cref="IDiscoverables"/> which delegates all member access to the <see cref="Inner"/> implementation,
/// unless the member is overridden in a derived class which changes the behavior.
/// </summary>
public class DiscoverablesPluginBase : PlayableCollectionGroupPluginBase, IModelPlugin, IDiscoverables, IDelegatable<IDiscoverables>
{
    /// <summary>
    /// Creates a new instance of <see cref="PlayableCollectionGroupPluginBase"/>.
    /// </summary>
    /// <param name="registration">Metadata about the plugin which was provided during registration.</param>
    /// <param name="inner">The implementation which all member access is delegated to, unless the member is overridden in a derived class which changes the behavior.</param>
    protected internal DiscoverablesPluginBase(ModelPluginMetadata registration, IDiscoverables inner)
        : base(registration, inner)
    {
        Inner = inner;
    }

    /// <inheritdoc />
    public virtual bool Equals(ICoreDiscoverables other) => Inner.Equals(other);

    /// <inheritdoc/>
    public IReadOnlyList<ICoreDiscoverables> Sources => ((IMerged<ICoreDiscoverables>)Inner).Sources;

    /// <inheritdoc />
    public new IDiscoverables Inner { get; }
}