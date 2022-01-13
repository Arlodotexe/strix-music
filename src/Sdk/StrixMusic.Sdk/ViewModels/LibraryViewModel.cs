using System.Collections.Generic;
using System.Threading.Tasks;
using OwlCore.Provisos;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.Models;
using StrixMusic.Sdk.Models.Base;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Models.Merged;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// A bindable wrapper of the <see cref="ILibraryBase"/>.
    /// </summary>
    public class LibraryViewModel : PlayableCollectionGroupViewModel, ILibrary, IAsyncInit
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

        /// <inheritdoc />
        public override Task InitAsync()
        {
            IsInitialized = true;

            // TODO sync library completely or pull from cache
            return Task.CompletedTask;
        }
    }
}
