﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OwlCore.ComponentModel;
using StrixMusic.Sdk.Models;
using StrixMusic.Sdk.Models.Core;

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
        protected internal UrlPluginBase(ModelPluginMetadata registration, IUrl inner)
        {
            Registration = registration;
            Inner = inner;
        }

        /// <inheritdoc />
        public ModelPluginMetadata Registration { get; }

        /// <inheritdoc/>
        public IUrl Inner { get; }

        /// <inheritdoc/>
        public virtual ValueTask DisposeAsync() => Inner.DisposeAsync();

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
        public IReadOnlyList<ICore> SourceCores => Inner.SourceCores;
    }
}