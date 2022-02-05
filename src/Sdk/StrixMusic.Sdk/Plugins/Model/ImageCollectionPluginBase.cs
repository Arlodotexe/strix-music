using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OwlCore.ComponentModel;
using OwlCore.Events;
using StrixMusic.Sdk.Models;
using StrixMusic.Sdk.Models.Core;

namespace StrixMusic.Sdk.Plugins.Model
{
    /// <summary>
    /// An implementation of <see cref="IImageCollection"/> which delegates all member access to the <see cref="Inner"/> implementation,
    /// unless the member is overridden in a derived class which changes the behavior.
    /// </summary>
    public abstract class ImageCollectionPluginBase : IImageCollection, IDelegatable<IImageCollection>
    {
        /// <summary>
        /// Creates a new instance of <see cref="DownloadablePluginBase"/>.
        /// </summary>
        /// <param name="registration">Metadata about the plugin which was provided during registration.</param>
        /// <param name="inner">The implementation which all member access is delegated to, unless the member is overridden in a derived class which changes the behavior.</param>
        protected ImageCollectionPluginBase(ModelPluginMetadata registration, IImageCollection inner)
        {
            Registration = registration;
            Inner = inner;
        }

        /// <inheritdoc />
        public ModelPluginMetadata Registration { get; }

        /// <inheritdoc/>
        public IImageCollection Inner { get; set; }

        /// <inheritdoc/>
        public virtual int TotalImageCount => Inner.TotalImageCount;

        /// <inheritdoc/>
        public virtual IReadOnlyList<ICoreImageCollection> Sources => Inner.Sources;

        /// <inheritdoc/>
        public virtual IReadOnlyList<ICore> SourceCores => Inner.SourceCores;

        /// <inheritdoc/>
        public virtual event CollectionChangedEventHandler<IImage>? ImagesChanged
        {
            add => Inner.ImagesChanged += value;
            remove => Inner.ImagesChanged -= value;
        }

        /// <inheritdoc/>
        public virtual event EventHandler<int>? ImagesCountChanged
        {
            add => Inner.ImagesCountChanged += value;
            remove => Inner.ImagesCountChanged -= value;
        }

        /// <inheritdoc/>
        public virtual Task AddImageAsync(IImage image, int index) => Inner.AddImageAsync(image, index);

        /// <inheritdoc/>
        public virtual ValueTask DisposeAsync() => Inner.DisposeAsync();

        /// <inheritdoc/>
        public virtual bool Equals(ICoreImageCollection other) => Inner.Equals(other);

        /// <inheritdoc/>
        public virtual Task<IReadOnlyList<IImage>> GetImagesAsync(int limit, int offset) => Inner.GetImagesAsync(limit, offset);

        /// <inheritdoc/>
        public virtual Task<bool> IsAddImageAvailableAsync(int index) => Inner.IsAddImageAvailableAsync(index);

        /// <inheritdoc/>
        public virtual Task<bool> IsRemoveImageAvailableAsync(int index) => Inner.IsRemoveImageAvailableAsync(index);

        /// <inheritdoc/>
        public virtual Task RemoveImageAsync(int index) => Inner.RemoveImageAsync(index);
    }
}
