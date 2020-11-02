using System.Collections.Generic;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Extensions.SdkMember;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// Used to bind search results across multiple cores to the View model.
    /// </summary>
    public class SearchResultsViewModel : PlayableCollectionGroupViewModel, ISearchResults
    {
        /// <summary>
        /// Bindable wrapper for <see cref="ISearchResults"/>.
        /// </summary>
        public SearchResultsViewModel(ISearchResults searchResults)
            : base(searchResults)
        {
        }

        /// <inheritdoc />
        IReadOnlyList<ICoreSearchResults> ISdkMember<ICoreSearchResults>.Sources => this.GetSources<ICoreSearchResults>();
    }
}
