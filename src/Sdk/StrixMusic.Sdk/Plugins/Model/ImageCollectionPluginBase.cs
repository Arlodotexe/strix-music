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
    /// An implementation of <see cref="IImageCollection"/> which delegates all member access to the <see cref="Inner"/> implementation,
    /// unless the member is overridden in a derived class which changes the behavior.
    /// </summary>
    public class ImageCollectionPluginBase : IModelPlugin, IImageCollection, IDelegatable<IImageCollection>
    {
        /// <summary>
        /// Creates a new instance of <see cref="ImageCollectionPluginBase"/>.
        /// </summary>
        /// <param name="registration">Metadata about the plugin which was provided during registration.</param>
        /// <param name="inner">The implementation which all member access is delegated to, unless the member is overridden in a derived class which changes the behavior.</param>
        protected internal ImageCollectionPluginBase(ModelPluginMetadata registration, IImageCollection inner)
        {
            Registration = registration;
            Inner = inner;
        }

        /// <inheritdoc />
        public ModelPluginMetadata Registration { get; }

        /// <inheritdoc/>
        public IImageCollection Inner { get; set; }

        /// <inheritdoc/>
        virtual public int TotalImageCount => Inner.TotalImageCount;

        /// <inheritdoc/>
        public IReadOnlyList<ICoreImageCollection> Sources => Inner.Sources;

        /// <inheritdoc/>
        public IReadOnlyList<ICore> SourceCores => Inner.SourceCores;

        /// <inheritdoc/>
        virtual public event CollectionChangedEventHandler<IImage>? ImagesChanged
        {
            add => Inner.ImagesChanged += value;
            remove => Inner.ImagesChanged -= value;
        }

        /// <inheritdoc/>
        virtual public event EventHandler<int>? ImagesCountChanged
        {
            add => Inner.ImagesCountChanged += value;
            remove => Inner.ImagesCountChanged -= value;
        }

        /// <inheritdoc/>
        virtual public Task AddImageAsync(IImage image, int index) => Inner.AddImageAsync(image, index);

        /// <inheritdoc/>
        virtual public ValueTask DisposeAsync() => Inner.DisposeAsync();

        /// <inheritdoc/>
        virtual public bool Equals(ICoreImageCollection other) => Inner.Equals(other);

        /// <inheritdoc/>
        virtual public Task<IReadOnlyList<IImage>> GetImagesAsync(int limit, int offset) => Inner.GetImagesAsync(limit, offset);

        /// <inheritdoc/>
        virtual public Task<bool> IsAddImageAvailableAsync(int index) => Inner.IsAddImageAvailableAsync(index);

        /// <inheritdoc/>
        virtual public Task<bool> IsRemoveImageAvailableAsync(int index) => Inner.IsRemoveImageAvailableAsync(index);

        /// <inheritdoc/>
        virtual public Task RemoveImageAsync(int index) => Inner.RemoveImageAsync(index);
    }
}
