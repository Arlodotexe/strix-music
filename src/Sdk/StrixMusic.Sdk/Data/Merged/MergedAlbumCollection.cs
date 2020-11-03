using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OwlCore.Collections;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Extensions.SdkMember;

namespace StrixMusic.Sdk.Data.Merged
{
    /// <summary>
    /// Merged multiple <see cref="ICoreAlbumCollection"/> into a single <see cref="IAlbumCollection"/>
    /// </summary>
    public class MergedAlbumCollection : IAlbumCollection, IMerged<ICoreAlbumCollection>
    {
        private readonly List<ICoreAlbumCollection> _sources;

        /// <summary>
        /// Creates a new instance of <see cref="MergedAlbumCollection"/>.
        /// </summary>
        /// <param name="sources">The initial sources to merge together.</param>
        public MergedAlbumCollection(IReadOnlyList<ICoreAlbumCollection> sources)
        {
            _sources = sources.ToList();
            Images = new SynchronizedObservableCollection<IImage>();
        }

        /// <inheritdoc cref="ISdkMember{T}.SourceCores" />
        public IReadOnlyList<ICore> SourceCores => _sources.Select(x => x.SourceCore).ToList();

        /// <inheritdoc cref="IMerged{T}.Sources"/>
        public IReadOnlyList<ICoreAlbumCollection> Sources => _sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreAlbumCollectionItem> ISdkMember<ICoreAlbumCollectionItem>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreAlbumCollection> ISdkMember<ICoreAlbumCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreImageCollection> ISdkMember<ICoreImageCollection>.Sources => Sources;

        /// <inheritdoc />
        public int TotalAlbumItemsCount { get; }

        /// <inheritdoc />
        public SynchronizedObservableCollection<IImage> Images { get; }

        /// <inheritdoc />
        public Task<bool> IsAddAlbumItemSupported(int index)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveAlbumItemSupported(int index)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public Task<bool> IsAddImageSupported(int index)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveImageSupported(int index)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public Task AddAlbumItemAsync(IAlbumCollectionItem album, int index)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public Task RemoveAlbumItemAsync(int index)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<IAlbumCollectionItem>> GetAlbumItemsAsync(int limit, int offset)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public void AddSource(ICoreAlbumCollection itemToMerge)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        public bool Equals(ICoreAlbumCollection other)
        {
            return other.Name.Equals(this.Name, StringComparison.InvariantCulture);
        }
    }
}