// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OwlCore.ComponentModel;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;

namespace StrixMusic.Sdk.Plugins.Model
{
    /// <summary>
    /// An implementation of <see cref="IUrl"/> which delegates all member access to the <see cref="Inner"/> implementation,
    /// unless the member is overridden in a derived class which changes the behavior.
    /// </summary>
    public class UrlPluginBase : IModelPlugin, IUrl, IDelegatable<IUrl>
    {
        /// <summary>
        /// Creates a new instance of <see cref="UrlPluginBase"/>.
        /// </summary>
        /// <param name="registration">Metadata about the plugin which was provided during registration.</param>
        /// <param name="inner">The implementation which all member access is delegated to, unless the member is overridden in a derived class which changes the behavior.</param>
        internal protected UrlPluginBase(ModelPluginMetadata registration, IUrl inner)
        {
            Metadata = registration;
            Inner = inner;
        }

        /// <inheritdoc />
        public ModelPluginMetadata Metadata { get; }

        /// <inheritdoc/>
        public IUrl Inner { get; }

        /// <inheritdoc/>
        public virtual string Label => Inner.Label;

        /// <inheritdoc/>
        public virtual Uri Url => Inner.Url;

        /// <inheritdoc/>
        public virtual UrlType Type => Inner.Type;

        /// <inheritdoc/>
        public virtual bool Equals(ICoreUrl other) => Inner.Equals(other);

        /// <inheritdoc/>
        public IReadOnlyList<ICoreUrl> Sources => Inner.Sources;

        /// <inheritdoc/>
        public event EventHandler? SourcesChanged
        {
            add => Inner.SourcesChanged += value;
            remove => Inner.SourcesChanged -= value;
        }
    }
}
