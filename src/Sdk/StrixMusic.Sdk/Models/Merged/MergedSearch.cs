using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.Extensions;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Services;

namespace StrixMusic.Sdk.Models.Merged
{
    /// <summary>
    /// Aggregates many <see cref="ICoreSearch"/> instances into one.
    /// </summary>
    public sealed class MergedSearch : ISearch, IMergedMutable<ICoreSearch>
    {
        private readonly List<ICoreSearch> _sources;
        private readonly List<ICore> _sourceCores;
        private readonly ISettingsService _settingsService;

        /// <summary>
        /// Creates a new instance of <see cref="MergedSearch"/>.
        /// </summary>
        public MergedSearch(IReadOnlyList<ICoreSearch> sources, ISettingsService settingsService)
        {
            _sources = sources.ToList();
            _sourceCores = Sources.Select(x => x.SourceCore).ToList();

            if (Sources.Any(x => x.SearchHistory != null))
                SearchHistory = new MergedSearchHistory(Sources.Select(x => x.SearchHistory).PruneNull(), settingsService);
            _settingsService = settingsService;
        }

        /// <inheritdoc />
        public IReadOnlyList<ICoreSearch> Sources => _sources;

        /// <inheritdoc />
        public IReadOnlyList<ICore> SourceCores => _sourceCores;

        /// <inheritdoc />
        public async IAsyncEnumerable<string> GetSearchAutoCompleteAsync(string query)
        {
            var sources = await Sources.InParallel(async x => await x.GetSearchAutoCompleteAsync(query).ToListAsync());

            foreach (var result in sources.SelectMany(x => x))
            {
                yield return result;
            }
        }

        /// <inheritdoc />
        public async Task<ISearchResults> GetSearchResultsAsync(string query)
        {
            var results = await Sources.InParallel(x => x.GetSearchResultsAsync(query));

            var merged = new MergedSearchResults(results, _settingsService);

            return merged;
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<ISearchQuery> GetRecentSearchQueries()
        {
            var sources = await Sources.InParallel(async x => await x.GetRecentSearchQueries().ToListAsync());

            var results = sources.SelectMany(x => x);

            var mergedData = new List<MergedSearchQuery>();

            foreach (var item in results)
            {
                // Inserts the new instance into its proper place by date.
                // Should minimize the amount of iterations and shifting of items in the list
                for (var i = mergedData.Count - 1; i >= 0; i--)
                {
                    // If we're at the start of the list, indiscriminately add to the collection.
                    if (i == 0)
                    {
                        mergedData.InsertOrAdd(i, new MergedSearchQuery(item.IntoList()));
                        break;
                    }

                    if (mergedData[i].Equals(item))
                    {
                        mergedData[i].Cast<IMergedMutable<ICoreSearchQuery>>().AddSource(item);
                        break;
                    }

                    if (item.CreatedAt > mergedData[i].CreatedAt)
                    {
                        mergedData.InsertOrAdd(i, new MergedSearchQuery(item.IntoList()));
                        break;
                    }
                }
            }

            foreach (var item in mergedData)
            {
                yield return item;
            }
        }

        /// <inheritdoc />
        public ISearchHistory? SearchHistory { get; }

        /// <inheritdoc />
        void IMergedMutable<ICoreSearch>.AddSource(ICoreSearch itemToMerge)
        {
            Guard.IsNotNull(itemToMerge, nameof(itemToMerge));

            _sources.Add(itemToMerge);
            _sourceCores.Add(itemToMerge.SourceCore);
        }

        /// <inheritdoc />
        void IMergedMutable<ICoreSearch>.RemoveSource(ICoreSearch itemToRemove)
        {
            Guard.IsNotNull(itemToRemove, nameof(itemToRemove));

            _sources.Remove(itemToRemove);
            _sourceCores.Remove(itemToRemove.SourceCore);
        }

        /// <inheritdoc />
        public bool Equals(ICoreSearch other)
        {
            // We always merge together search sources.
            return true;
        }

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            await _sources.InParallel(x => x.DisposeAsync().AsTask());
        }
    }
}