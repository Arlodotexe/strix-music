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
    /// An implementation of <see cref="IGenreCollection"/> which delegates all member access to the <see cref="Inner"/> implementation,
    /// unless the member is overridden in a derived class which changes the behavior.
    /// </summary>
    public class GenreCollectionPluginBase : IModelPlugin, IGenreCollection, IDelegatable<IGenreCollection>
    {
        /// <summary>
        /// Creates a new instance of <see cref="GenreCollectionPluginBase"/>.
        /// </summary>
        /// <param name="registration">Metadata about the plugin which was provided during registration.</param>
        /// <param name="inner">The implementation which all member access is delegated to, unless the member is overridden in a derived class which changes the behavior.</param>
        internal protected GenreCollectionPluginBase(ModelPluginMetadata registration, IGenreCollection inner)
        {
            Metadata = registration;
            Inner = inner;
        }

        /// <inheritdoc />
        public ModelPluginMetadata Metadata { get; }

        /// <inheritdoc/>
        public IGenreCollection Inner { get; set; }

        /// <inheritdoc/>
        public event EventHandler? SourcesChanged
        {
            add => Inner.SourcesChanged += value;
            remove => Inner.SourcesChanged -= value;
        }

        /// <inheritdoc/>
        public virtual int TotalGenreCount => Inner.TotalGenreCount;

        /// <inheritdoc/>
        public IReadOnlyList<ICoreGenreCollection> Sources => Inner.Sources;

        /// <inheritdoc/>
        public virtual event CollectionChangedEventHandler<IGenre>? GenresChanged
        {
            add => Inner.GenresChanged += value;
            remove => Inner.GenresChanged -= value;
        }

        /// <inheritdoc/>
        public virtual event EventHandler<int>? GenresCountChanged
        {
            add => Inner.GenresCountChanged += value;
            remove => Inner.GenresCountChanged -= value;
        }

        /// <inheritdoc/>
        public virtual Task AddGenreAsync(IGenre gene, int index, CancellationToken cancellationToken = default) => Inner.AddGenreAsync(gene, index, cancellationToken);

        /// <inheritdoc/>
        public virtual bool Equals(ICoreGenreCollection other) => Inner.Equals(other);

        /// <inheritdoc/>
        public virtual IAsyncEnumerable<IGenre> GetGenresAsync(int limit, int offset, CancellationToken cancellationToken = default) => Inner.GetGenresAsync(limit, offset, cancellationToken);

        /// <inheritdoc/>
        public virtual Task<bool> IsAddGenreAvailableAsync(int index, CancellationToken cancellationToken = default) => Inner.IsAddGenreAvailableAsync(index, cancellationToken);

        /// <inheritdoc/>
        public virtual Task<bool> IsRemoveGenreAvailableAsync(int index, CancellationToken cancellationToken = default) => Inner.IsRemoveGenreAvailableAsync(index, cancellationToken);

        /// <inheritdoc/>
        public virtual Task RemoveGenreAsync(int index, CancellationToken cancellationToken = default) => Inner.RemoveGenreAsync(index, cancellationToken);
    }
}
