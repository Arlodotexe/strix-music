using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OwlCore.ComponentModel;
using OwlCore.Events;
using StrixMusic.Sdk.Models;
using StrixMusic.Sdk.Models.Core;

namespace StrixMusic.Sdk.Plugins.Model
{
    /// <summary>
    /// An implementation of <see cref="IUrlCollection"/> which delegates all member access to the <see cref="Inner"/> implementation,
    /// unless the member is overridden in a derived class which changes the behavior.
    /// </summary>
    public abstract class UrlCollectionPluginBase : IUrlCollection, IDelegatable<IUrlCollection>
    {
        /// <summary>
        /// Creates a new instance of <see cref="DownloadablePluginBase"/>.
        /// </summary>
        /// <param name="registration">Metadata about the plugin which was provided during registration.</param>
        /// <param name="inner">The implementation which all member access is delegated to, unless the member is overridden in a derived class which changes the behavior.</param>
        protected UrlCollectionPluginBase(ModelPluginMetadata registration, IUrlCollection inner)
        {
            Registration = registration;
            Inner = inner;
        }

        /// <inheritdoc />
        public ModelPluginMetadata Registration { get; }

        /// <inheritdoc/>
        public IUrlCollection Inner { get; set; }

        /// <inheritdoc/>
        public virtual int TotalUrlCount => Inner.TotalUrlCount;

        /// <inheritdoc/>
        public IReadOnlyList<ICoreUrlCollection> Sources => Inner.Sources;

        /// <inheritdoc/>
        public IReadOnlyList<ICore> SourceCores => Inner.SourceCores;

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
        public virtual Task AddUrlAsync(IUrl url, int index) => Inner.AddUrlAsync(url, index);

        /// <inheritdoc/>
        public virtual ValueTask DisposeAsync() => Inner.DisposeAsync();

        /// <inheritdoc/>
        public virtual bool Equals(ICoreUrlCollection other) => Inner.Equals(other);

        /// <inheritdoc/>
        public virtual Task<IReadOnlyList<IUrl>> GetUrlsAsync(int limit, int offset) => Inner.GetUrlsAsync(limit, offset);

        /// <inheritdoc/>
        public virtual Task<bool> IsAddUrlAvailableAsync(int index) => Inner.IsAddUrlAvailableAsync(index);

        /// <inheritdoc/>
        public virtual Task<bool> IsRemoveUrlAvailableAsync(int index) => Inner.IsRemoveUrlAvailableAsync(index);

        /// <inheritdoc/>
        public virtual Task RemoveUrlAsync(int index) => Inner.RemoveUrlAsync(index);
    }
}
