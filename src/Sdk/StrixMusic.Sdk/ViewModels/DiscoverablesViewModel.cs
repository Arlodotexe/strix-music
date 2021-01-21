using System.Collections.Generic;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Data.Merged;
using StrixMusic.Sdk.Extensions;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// A bindable wrapper of the <see cref="IDiscoverables"/>.
    /// </summary>
    public class DiscoverablesViewModel : PlayableCollectionGroupViewModel, IDiscoverables
    {
        private readonly IDiscoverables _discoverables;

        /// <summary>
        /// Creates a new instance of the <see cref="DiscoverablesViewModel"/> class.
        /// </summary>
        /// <param name="discoverables">The <see cref="IDiscoverables"/> to wrap.</param>
        public DiscoverablesViewModel(IDiscoverables discoverables)
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