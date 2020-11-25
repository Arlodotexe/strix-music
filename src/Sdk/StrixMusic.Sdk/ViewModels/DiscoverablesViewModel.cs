using System.Collections.Generic;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Extensions;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// A bindable wrapper of the <see cref="IDiscoverables"/>.
    /// </summary>
    public class DiscoverablesViewModel : PlayableCollectionGroupViewModel, IDiscoverables
    {
        /// <summary>
        /// Creates a new instance of the <see cref="DiscoverablesViewModel"/> class.
        /// </summary>
        /// <param name="discoverables">The <see cref="IDiscoverables"/> to wrap.</param>
        public DiscoverablesViewModel(IDiscoverables discoverables)
            : base(discoverables)
        {
        }

        /// <inheritdoc />
        IReadOnlyList<ICoreDiscoverables> ISdkMember<ICoreDiscoverables>.Sources => this.GetSources<ICoreDiscoverables>();
    }
}