// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OwlCore.ComponentModel;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;

namespace StrixMusic.Sdk.Plugins.Model
{
    /// <summary>
    /// An implementation of <see cref="IUrlCollection"/> which delegates all member access to the <see cref="Inner"/> implementation,
    /// unless the member is overridden in a derived class which changes the behavior.
    /// </summary>
    public class UrlCollectionPluginBase : IModelPlugin, IUrlCollection, IDelegatable<IUrlCollection>
    {
        /// <summary>
        /// Creates a new instance of <see cref="UrlCollectionPluginBase"/>.
        /// </summary>
        /// <param name="registration">Metadata about the plugin which was provided during registration.</param>
        /// <param name="inner">The implementation which all member access is delegated to, unless the member is overridden in a derived class which changes the behavior.</param>
        internal protected UrlCollectionPluginBase(ModelPluginMetadata registration, IUrlCollection inner)
        {
            Metadata = registration;
            Inner = inner;
        }

        /// <inheritdoc />
        public ModelPluginMetadata Metadata { get; }

        /// <inheritdoc/>
        public IUrlCollection Inner { get; set; }

        /// <inheritdoc/>
        public virtual int TotalUrlCount => Inner.TotalUrlCount;

        /// <inheritdoc/>
        public IReadOnlyList<ICoreUrlCollection> Sources => Inner.Sources;

        /// <inheritdoc/>
        public event EventHandler? SourcesChanged
        {
            add => Inner.SourcesChanged += value;
            remove => Inner.SourcesChanged -= value;
        }

        /// <inheritdoc/>
        public virtual event CollectionChangedEventHandler<IUrl>? UrlsChanged
        {
            add => Inner.UrlsChanged += value;
            remove => Inner.UrlsChanged -= value;
        }

        /// <inheritdoc/>
        public virtual event EventHandler<int>? UrlsCountChanged
        {
            add => Inner.UrlsCountChanged += value;
            remove => Inner.UrlsCountChanged -= value;
        }

        /// <inheritdoc/>
        public virtual Task AddUrlAsync(IUrl url, int index, CancellationToken cancellationToken = default) => Inner.AddUrlAsync(url, index, cancellationToken);

        /// <inheritdoc/>
        public virtual bool Equals(ICoreUrlCollection other) => Inner.Equals(other);

        /// <inheritdoc/>
        public virtual IAsyncEnumerable<IUrl> GetUrlsAsync(int limit, int offset, CancellationToken cancellationToken = default) => Inner.GetUrlsAsync(limit, offset, cancellationToken);

        /// <inheritdoc/>
        public virtual Task<bool> IsAddUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => Inner.IsAddUrlAvailableAsync(index, cancellationToken);

        /// <inheritdoc/>
        public virtual Task<bool> IsRemoveUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => Inner.IsRemoveUrlAvailableAsync(index, cancellationToken);

        /// <inheritdoc/>
        public virtual Task RemoveUrlAsync(int index, CancellationToken cancellationToken = default) => Inner.RemoveUrlAsync(index, cancellationToken);
    }
}
