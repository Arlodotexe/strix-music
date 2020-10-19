using StrixMusic.Sdk.Core.Data;

namespace StrixMusic.Sdk.Core.ViewModels
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
    }
}