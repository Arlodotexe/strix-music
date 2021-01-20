using System.Collections.Generic;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Data.Merged;
using StrixMusic.Sdk.Extensions;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// A bindable wrapper of the <see cref="ILibraryBase"/>.
    /// </summary>
    public class LibraryViewModel : PlayableCollectionGroupViewModel, ILibrary
    {
        private readonly ILibrary _library;

        /// <summary>
        /// Creates a new instance of the <see cref="LibraryViewModel"/> class.
        /// </summary>
        /// <param name="library">The <see cref="ILibrary"/> to wrap.</param>
        public LibraryViewModel(ILibrary library)
            : base(library)
        {
            _library = library;
        }

        /// <inheritdoc />
        IReadOnlyList<ICoreLibrary> IMerged<ICoreLibrary>.Sources => this.GetSources<ICoreLibrary>();

        /// <inheritdoc />
        public bool Equals(ICoreLibrary other) => _library.Equals(other);
    }
}
