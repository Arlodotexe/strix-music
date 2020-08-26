using System.Collections.ObjectModel;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using StrixMusic.CoreInterfaces.Interfaces;

namespace StrixMusic.ViewModels.Bindables
{
    /// <summary>
    /// Used to bind search results across multiple cores to the View model.
    /// </summary>
    public class BindableSearchResults : BindableCollectionGroup
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public BindableSearchResults(ISearchResults searchResults)
            : base(searchResults)
        {
        }
    }
}
