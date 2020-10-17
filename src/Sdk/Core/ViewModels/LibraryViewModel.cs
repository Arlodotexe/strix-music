using StrixMusic.Sdk.Core.Data;

namespace StrixMusic.Sdk.Core.ViewModels
{
    /// <summary>
    /// A bindable wrapper of the <see cref="ILibrary"/>.
    /// </summary>
    public class LibraryViewModel : PlayableCollectionGroupViewModel, ILibrary
    {
        /// <summary>
        /// Creates a new instance of the <see cref="LibraryViewModel"/> class.
        /// </summary>
        /// <param name="library">The <see cref="ILibrary"/> to wrap.</param>
        public LibraryViewModel(ILibrary library)
            : base(library)
        {
        }
    }
}
