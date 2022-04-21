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
    /// A ViewModel for <see cref="IRecentlyPlayed"/>.
    /// </summary>
    public sealed class RecentlyPlayedViewModel : PlayableCollectionGroupViewModel, ISdkViewModel, IRecentlyPlayed
    {
        private readonly IRecentlyPlayed _recentlyPlayed;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecentlyPlayedViewModel"/> class.
        /// </summary>
        /// <param name="root">The <see cref="MainViewModel"/> that this or the object that created this originated from.</param>
        /// <param name="recentlyPlayed">The <see cref="IRecentlyPlayed"/> to wrap.</param>
        internal RecentlyPlayedViewModel(MainViewModel root, IRecentlyPlayed recentlyPlayed)
            : base(root, recentlyPlayed)
        {
            _recentlyPlayed = recentlyPlayed;
        }

        /// <inheritdoc />
        IReadOnlyList<ICoreRecentlyPlayed> IMerged<ICoreRecentlyPlayed>.Sources => this.GetSources<ICoreRecentlyPlayed>();

        /// <inheritdoc />
        public bool Equals(ICoreRecentlyPlayed other) => _recentlyPlayed.Equals(other);
    }
}
