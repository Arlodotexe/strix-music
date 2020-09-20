using StrixMusic.Sdk.Interfaces;

namespace StrixMusic.Sdk.Observables
{
    /// <summary>
    /// A bindable wrapper of the <see cref="ILibrary"/>.
    /// </summary>
    public class ObservableLibrary : ObservableCollectionGroup, ILibrary
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ObservableLibrary"/> class.
        /// </summary>
        /// <param name="library">The <see cref="ILibrary"/> to wrap.</param>
        public ObservableLibrary(ILibrary library)
            : base(library)
        {
        }
    }
}
