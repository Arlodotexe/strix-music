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
    public class GenreCollectionPluginBase : IModelPlugin, IGenreCollection, IDelegatable<IGenreCollection>
    {
        /// <summary>
        /// Creates a new instance of <see cref="GenreCollectionPluginBase"/>.
        /// </summary>
        /// <param name="registration">Metadata about the plugin which was provided during registration.</param>
        /// <param name="inner">The implementation which all member access is delegated to, unless the member is overridden in a derived class which changes the behavior.</param>
        protected internal GenreCollectionPluginBase(ModelPluginMetadata registration, IGenreCollection inner)
        {
            Metadata = registration;
            Inner = inner;
        }

        /// <inheritdoc />
        public ModelPluginMetadata Metadata { get; }

        /// <inheritdoc/>
        public IGenreCollection Inner { get; set; }

        /// <inheritdoc/>
        virtual public int TotalGenreCount => Inner.TotalGenreCount;

        /// <inheritdoc/>
        public IReadOnlyList<ICoreGenreCollection> Sources => Inner.Sources;

        /// <inheritdoc/>
        public IReadOnlyList<ICore> SourceCores => Inner.SourceCores;

        /// <inheritdoc/>
        virtual public event CollectionChangedEventHandler<IGenre>? GenresChanged
        {
            add => Inner.GenresChanged += value;
            remove => Inner.GenresChanged -= value;
        }

        /// <inheritdoc/>
        virtual public event EventHandler<int>? GenresCountChanged
        {
            add => Inner.GenresCountChanged += value;
            remove => Inner.GenresCountChanged -= value;
        }

        /// <inheritdoc/>
        virtual public Task AddGenreAsync(IGenre gene, int index) => Inner.AddGenreAsync(gene, index);

        /// <inheritdoc/>
        virtual public ValueTask DisposeAsync() => Inner.DisposeAsync();

        /// <inheritdoc/>
        virtual public bool Equals(ICoreGenreCollection other) => Inner.Equals(other);

        /// <inheritdoc/>
        virtual public Task<IReadOnlyList<IGenre>> GetGenresAsync(int limit, int offset) => Inner.GetGenresAsync(limit, offset);

        /// <inheritdoc/>
        virtual public Task<bool> IsAddGenreAvailableAsync(int index) => Inner.IsAddGenreAvailableAsync(index);

        /// <inheritdoc/>
        virtual public Task<bool> IsRemoveGenreAvailableAsync(int index) => Inner.IsRemoveGenreAvailableAsync(index);

        /// <inheritdoc/>
        virtual public Task RemoveGenreAsync(int index) => Inner.RemoveGenreAsync(index);
    }
}
