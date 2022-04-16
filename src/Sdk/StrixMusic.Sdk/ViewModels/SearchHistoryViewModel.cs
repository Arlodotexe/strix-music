// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System.Collections.Generic;
using StrixMusic.Sdk.AdapterModels;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.Models;
using StrixMusic.Sdk.Models.Core;

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
        /// <param name="root">The <see cref="MainViewModel"/> that this or the object that created this originated from.</param>
        /// <param name="searchHistory">The <see cref="ISearchHistory"/> to wrap.</param>
        internal SearchHistoryViewModel(MainViewModel root, ISearchHistory searchHistory)
            : base(root, searchHistory)
        {
            _searchHistory = root.Plugins.ModelPlugins.SearchHistory.Execute(searchHistory);
        }

        /// <inheritdoc />
        IReadOnlyList<ICoreSearchHistory> IMerged<ICoreSearchHistory>.Sources => this.GetSources<ICoreSearchHistory>();

        /// <inheritdoc />
        public bool Equals(ICoreSearchHistory other) => _searchHistory.Equals(other);
    }
}
