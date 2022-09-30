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
        /// <param name="pluginRoot">The plugin-enabled <see cref="IStrixDataRoot" /> which is responsible for creating this and all parent instances.</param>
        internal protected ImageCollectionPluginBase(ModelPluginMetadata registration, IImageCollection inner, IStrixDataRoot pluginRoot)
        {
            Metadata = registration;
            Inner = inner;
            Root = pluginRoot;
        }

        /// <inheritdoc />
        public ModelPluginMetadata Metadata { get; }

        /// <inheritdoc/>
        public IImageCollection Inner { get; set; }

        /// <inheritdoc/>
        public event EventHandler? SourcesChanged
        {
            add => Inner.SourcesChanged += value;
            remove => Inner.SourcesChanged -= value;
        }

        /// <inheritdoc/>
        public virtual int TotalImageCount => Inner.TotalImageCount;

        /// <inheritdoc/>
        public IReadOnlyList<ICoreImageCollection> Sources => Inner.Sources;

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
        public virtual Task AddImageAsync(IImage image, int index, CancellationToken cancellationToken = default) => Inner.AddImageAsync(image, index, cancellationToken);

        /// <inheritdoc/>
        public virtual bool Equals(ICoreImageCollection other) => Inner.Equals(other);

        /// <inheritdoc/>
        public virtual IAsyncEnumerable<IImage> GetImagesAsync(int limit, int offset, CancellationToken cancellationToken = default) => Inner.GetImagesAsync(limit, offset, cancellationToken);

        /// <inheritdoc/>
        public virtual Task<bool> IsAddImageAvailableAsync(int index, CancellationToken cancellationToken = default) => Inner.IsAddImageAvailableAsync(index, cancellationToken);

        /// <inheritdoc/>
        public virtual Task<bool> IsRemoveImageAvailableAsync(int index, CancellationToken cancellationToken = default) => Inner.IsRemoveImageAvailableAsync(index, cancellationToken);

        /// <inheritdoc/>
        public virtual Task RemoveImageAsync(int index, CancellationToken cancellationToken = default) => Inner.RemoveImageAsync(index, cancellationToken);

        /// <inheritdoc/>
        public IStrixDataRoot Root { get; }
    }
}
