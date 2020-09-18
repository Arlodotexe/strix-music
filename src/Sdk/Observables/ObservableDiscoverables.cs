using StrixMusic.Sdk.Interfaces;

namespace StrixMusic.Sdk.Observables
{
    /// <summary>
    /// A bindable wrapper of the <see cref="IDiscoverables"/>.
    /// </summary>
    public class ObservableDiscoverables : ObservableCollectionGroup
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ObservableDiscoverables"/> class.
        /// </summary>
        /// <param name="discoverables">The <see cref="IDiscoverables"/> to wrap.</param>
        public ObservableDiscoverables(IDiscoverables discoverables)
            : base(discoverables)
        {
        }
    }
}