using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// A bindable wrapper of the <see cref="ICoreDiscoverables"/>.
    /// </summary>
    public class DiscoverablesViewModel : PlayableCollectionGroupViewModel, ICoreDiscoverables
    {
        /// <summary>
        /// Creates a new instance of the <see cref="DiscoverablesViewModel"/> class.
        /// </summary>
        /// <param name="discoverables">The <see cref="ICoreDiscoverables"/> to wrap.</param>
        public DiscoverablesViewModel(ICoreDiscoverables discoverables)
            : base(discoverables)
        {
        }
    }
}