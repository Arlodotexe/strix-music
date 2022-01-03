using System.Collections.Generic;
using StrixMusic.Sdk.Models;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Models.Merged;
using StrixMusic.Sdk.Extensions;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// Used to bind search results across multiple cores to the View model.
    /// </summary>
    public class SearchResultsViewModel : PlayableCollectionGroupViewModel, ISearchResults
    {
        private readonly ISearchResults _searchResults;

        /// <summary>
        /// Bindable wrapper for <see cref="ISearchResults"/>.
        /// </summary>
        public SearchResultsViewModel(ISearchResults searchResults)
            : base(searchResults)
        {
            _searchResults = searchResults;
        }

        /// <inheritdoc />
        IReadOnlyList<ICoreSearchResults> IMerged<ICoreSearchResults>.Sources => this.GetSources<ICoreSearchResults>();

        /// <inheritdoc />
        public bool Equals(ICoreSearchResults other) => _searchResults.Equals(other);
    }
}
