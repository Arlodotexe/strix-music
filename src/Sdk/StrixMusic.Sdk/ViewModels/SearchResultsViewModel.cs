using System.Collections.Generic;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.Models;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Models.Merged;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// Used to bind to search results across multiple cores.
    /// </summary>
    public sealed class SearchResultsViewModel : PlayableCollectionGroupViewModel, ISdkViewModel, ISearchResults
    {
        private readonly ISearchResults _searchResults;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchResultsViewModel"/> class.
        /// </summary>
        /// <param name="root">The <see cref="MainViewModel"/> that this or the object that created this originated from.</param>
        /// <param name="searchResults">The <see cref="ISearchResults"/> to wrap.</param>
        internal SearchResultsViewModel(MainViewModel root, ISearchResults searchResults)
            : base(root, searchResults)
        {
            _searchResults = searchResults;
        }

        /// <inheritdoc />
        IReadOnlyList<ICoreSearchResults> IMerged<ICoreSearchResults>.Sources => this.GetSources<ICoreSearchResults>();

        /// <inheritdoc />
        public bool Equals(ICoreSearchResults other) => _searchResults.Equals(other);
    }
}
