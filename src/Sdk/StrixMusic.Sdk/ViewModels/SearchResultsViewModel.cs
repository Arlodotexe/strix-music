using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// Used to bind search results across multiple cores to the View model.
    /// </summary>
    public class SearchResultsViewModel : PlayableCollectionGroupViewModel
    {
        /// <summary>
        /// Bindable wrapper for <see cref="ICoreSearchResults"/>.
        /// </summary>
        public SearchResultsViewModel(ICoreSearchResults searchResults)
            : base(searchResults)
        {
        }
    }
}
