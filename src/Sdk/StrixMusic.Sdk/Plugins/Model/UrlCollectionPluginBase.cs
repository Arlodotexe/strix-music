using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OwlCore.ComponentModel;
using OwlCore.Events;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Sdk.Models;

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
        protected internal UrlCollectionPluginBase(ModelPluginMetadata registration, IUrlCollection inner)
        {
            Metadata = registration;
            Inner = inner;
        }

        /// <inheritdoc />
        public ModelPluginMetadata Metadata { get; }

        /// <inheritdoc/>
        public IUrlCollection Inner { get; set; }

        /// <inheritdoc/>
        virtual public int TotalUrlCount => Inner.TotalUrlCount;

        /// <inheritdoc/>
        public IReadOnlyList<ICoreUrlCollection> Sources => Inner.Sources;

        /// <inheritdoc/>
        public IReadOnlyList<ICore> SourceCores => Inner.SourceCores;

        /// <inheritdoc/>
        virtual public event CollectionChangedEventHandler<IUrl>? UrlsChanged
        {
            add => Inner.UrlsChanged += value;
            remove => Inner.UrlsChanged -= value;
        }

        /// <inheritdoc/>
        virtual public event EventHandler<int>? UrlsCountChanged
        {
            add => Inner.UrlsCountChanged += value;
            remove => Inner.UrlsCountChanged -= value;
        }

        /// <inheritdoc/>
        virtual public Task AddUrlAsync(IUrl url, int index, CancellationToken cancellationToken = default) => Inner.AddUrlAsync(url, index, cancellationToken);

        /// <inheritdoc/>
        virtual public ValueTask DisposeAsync() => Inner.DisposeAsync();

        /// <inheritdoc/>
        virtual public bool Equals(ICoreUrlCollection other) => Inner.Equals(other);

        /// <inheritdoc/>
        virtual public Task<IReadOnlyList<IUrl>> GetUrlsAsync(int limit, int offset, CancellationToken cancellationToken = default) => Inner.GetUrlsAsync(limit, offset, cancellationToken);

        /// <inheritdoc/>
        virtual public Task<bool> IsAddUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => Inner.IsAddUrlAvailableAsync(index, cancellationToken);

        /// <inheritdoc/>
        virtual public Task<bool> IsRemoveUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => Inner.IsRemoveUrlAvailableAsync(index, cancellationToken);

        /// <inheritdoc/>
        virtual public Task RemoveUrlAsync(int index, CancellationToken cancellationToken = default) => Inner.RemoveUrlAsync(index, cancellationToken);
    }
}
