// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System.Collections.Generic;
using StrixMusic.Sdk.AdapterModels;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Sdk.Extensions;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// A ViewModel for <see cref="ISearchResults"/>.
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
