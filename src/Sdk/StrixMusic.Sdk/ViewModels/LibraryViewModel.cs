using StrixMusic.Sdk.Core.Data;

namespace StrixMusic.Sdk.Core.ViewModels
{
    /// <summary>
    /// A bindable wrapper of the <see cref="ILibraryBase"/>.
    /// </summary>
    public class LibraryViewModel : PlayableCollectionGroupViewModel, ILibraryBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="LibraryViewModel"/> class.
        /// </summary>
        /// <param name="library">The <see cref="ILibraryBase"/> to wrap.</param>
        public LibraryViewModel(ILibraryBase library)
            : base(library)
        {
        }
    }
}
