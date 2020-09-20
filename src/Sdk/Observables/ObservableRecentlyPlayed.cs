using System.Collections.ObjectModel;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using StrixMusic.Sdk.Interfaces;

namespace StrixMusic.Sdk.Observables
{
    /// <summary>
    /// Used to bind recently played across multiple cores to the View model.
    /// </summary>
    public class ObservableRecentlyPlayed : ObservableCollectionGroup, IRecentlyPlayed
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableRecentlyPlayed"/> class.
        /// </summary>
        /// <param name="recentlyPlayed">The <see cref="IRecentlyPlayed"/> to wrap.</param>
        public ObservableRecentlyPlayed(IRecentlyPlayed recentlyPlayed)
            : base(recentlyPlayed)
        {
        }
    }
}
