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
    /// A ViewModel for <see cref="ISearchHistory"/>.
    /// </summary>
    public sealed class SearchHistoryViewModel : PlayableCollectionGroupViewModel, ISdkViewModel, ISearchHistory
    {
        private readonly ISearchHistory _searchHistory;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecentlyPlayedViewModel"/> class.
        /// </summary>
        /// <param name="searchHistory">The <see cref="ISearchHistory"/> to wrap.</param>
        /// <param name="viewModelRoot">The ViewModel-enabled <see cref="IStrixDataRoot" /> which is responsible for creating this and all parent instances.</param>
        public SearchHistoryViewModel(ISearchHistory searchHistory, IStrixDataRoot viewModelRoot)
            : base(searchHistory, viewModelRoot)
        {
            _searchHistory = searchHistory;
        }

        /// <inheritdoc />
        IReadOnlyList<ICoreSearchHistory> IMerged<ICoreSearchHistory>.Sources => this.GetSources<ICoreSearchHistory>();

        /// <inheritdoc />
        public bool Equals(ICoreSearchHistory other) => _searchHistory.Equals(other);
    }
}
