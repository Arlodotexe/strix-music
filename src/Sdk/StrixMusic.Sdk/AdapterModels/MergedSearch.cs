// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using OwlCore.Extensions;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;

namespace StrixMusic.Sdk.AdapterModels
{
    /// <summary>
    /// Aggregates many <see cref="ICoreSearch"/> instances into one.
    /// </summary>
    public sealed class MergedSearch : ISearch, IMergedMutable<ICoreSearch>
    {
        private readonly MergedCollectionConfig _config;
        private readonly List<ICoreSearch> _sources;

        /// <summary>
        /// Creates a new instance of <see cref="MergedSearch"/>.
        /// </summary>
        public MergedSearch(IReadOnlyList<ICoreSearch> sources, MergedCollectionConfig config)
        {
            _config = config;
            _sources = sources.ToList();

            if (Sources.Any(x => x.SearchHistory != null))
                SearchHistory = new MergedSearchHistory(Sources.Select(x => x.SearchHistory).PruneNull(), config);
        }
        
        /// <inheritdoc cref="IMerged.SourcesChanged"/>
        public event EventHandler? SourcesChanged;

        /// <inheritdoc />
        public IReadOnlyList<ICoreSearch> Sources => _sources;

        /// <inheritdoc />
        public async IAsyncEnumerable<string> GetSearchAutoCompleteAsync(string query, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var sources = await Sources.InParallel(async x => await x.GetSearchAutoCompleteAsync(query, cancellationToken).ToListAsync(cancellationToken));

            foreach (var result in sources.SelectMany(x => x))
            {
                yield return result;
            }
        }

        /// <inheritdoc />
        public async Task<ISearchResults> GetSearchResultsAsync(string query, CancellationToken cancellationToken = default)
        {
            var results = await Sources.InParallel(x => x.GetSearchResultsAsync(query, cancellationToken));

            var merged = new MergedSearchResults(results, _config);

            return merged;
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<ISearchQuery> GetRecentSearchQueries([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var sources = await Sources.InParallel(async x => await x.GetRecentSearchQueries(cancellationToken).ToListAsync(cancellationToken));

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
                        mergedData[i].AddSource(item);
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
        public void AddSource(ICoreSearch itemToMerge)
        {
            Guard.IsNotNull(itemToMerge, nameof(itemToMerge));

            _sources.Add(itemToMerge);
            SourcesChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <inheritdoc />
        public void RemoveSource(ICoreSearch itemToRemove)
        {
            Guard.IsNotNull(itemToRemove, nameof(itemToRemove));

            _sources.Remove(itemToRemove);
            SourcesChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true. We always merge together search sources.</returns>
        public bool Equals(ICoreSearch other) => true;

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            await _sources.InParallel(x => x.DisposeAsync().AsTask());
        }
    }
}
