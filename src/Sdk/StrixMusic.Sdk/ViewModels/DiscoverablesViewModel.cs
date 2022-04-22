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
    /// A ViewModel for <see cref="IDiscoverables"/>.
    /// </summary>
    public class DiscoverablesViewModel : PlayableCollectionGroupViewModel, ISdkViewModel, IDiscoverables
    {
        private readonly IDiscoverables _discoverables;

        /// <summary>
        /// Creates a new instance of the <see cref="DiscoverablesViewModel"/> class.
        /// </summary>
        /// <param name="discoverables">The <see cref="IDiscoverables"/> to wrap.</param>
        internal DiscoverablesViewModel(IDiscoverables discoverables)
            : base(discoverables)
        {
            _discoverables = discoverables;
        }

        /// <inheritdoc />
        IReadOnlyList<ICoreDiscoverables> IMerged<ICoreDiscoverables>.Sources => this.GetSources<ICoreDiscoverables>();

        /// <inheritdoc />
        public bool Equals(ICoreDiscoverables other) => _discoverables.Equals(other);
    }
}
