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
    /// An implementation of <see cref="IGenreCollection"/> which delegates all member access to the <see cref="Inner"/> implementation,
    /// unless the member is overridden in a derived class which changes the behavior.
    /// </summary>
    public abstract class GenreCollectionPluginBase : IModelPlugin, IGenreCollection, IDelegatable<IGenreCollection>
    {
        /// <summary>
        /// Creates a new instance of <see cref="DownloadablePluginBase"/>.
        /// </summary>
        /// <param name="registration">Metadata about the plugin which was provided during registration.</param>
        /// <param name="inner">The implementation which all member access is delegated to, unless the member is overridden in a derived class which changes the behavior.</param>
        protected GenreCollectionPluginBase(ModelPluginMetadata registration, IGenreCollection inner)
        {
            Registration = registration;
            Inner = inner;
        }

        /// <inheritdoc />
        public ModelPluginMetadata Registration { get; }

        /// <inheritdoc/>
        public IGenreCollection Inner { get; set; }

        /// <inheritdoc/>
        public virtual int TotalGenreCount => Inner.TotalGenreCount;

        /// <inheritdoc/>
        public IReadOnlyList<ICoreGenreCollection> Sources => Inner.Sources;

        /// <inheritdoc/>
        public IReadOnlyList<ICore> SourceCores => Inner.SourceCores;

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
        public virtual Task AddGenreAsync(IGenre gene, int index) => Inner.AddGenreAsync(gene, index);

        /// <inheritdoc/>
        public virtual ValueTask DisposeAsync() => Inner.DisposeAsync();

        /// <inheritdoc/>
        public virtual bool Equals(ICoreGenreCollection other) => Inner.Equals(other);

        /// <inheritdoc/>
        public virtual Task<IReadOnlyList<IGenre>> GetGenresAsync(int limit, int offset) => Inner.GetGenresAsync(limit, offset);

        /// <inheritdoc/>
        public virtual Task<bool> IsAddGenreAvailableAsync(int index) => Inner.IsAddGenreAvailableAsync(index);

        /// <inheritdoc/>
        public virtual Task<bool> IsRemoveGenreAvailableAsync(int index) => Inner.IsRemoveGenreAvailableAsync(index);

        /// <inheritdoc/>
        public virtual Task RemoveGenreAsync(int index) => Inner.RemoveGenreAsync(index);
    }
}
