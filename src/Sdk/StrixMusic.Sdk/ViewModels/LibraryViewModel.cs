// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System.Collections.Generic;
using OwlCore.ComponentModel;
using StrixMusic.Sdk.AdapterModels;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Sdk.Extensions;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// A ViewModel for <see cref="ILibrary"/>.
    /// </summary>
    public sealed class LibraryViewModel : PlayableCollectionGroupViewModel, ISdkViewModel, ILibrary, IDelegatable<ILibrary>
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

        /// <inheritdoc/>
        ILibrary IDelegatable<ILibrary>.Inner => _library;

        /// <inheritdoc />
        IReadOnlyList<ICoreLibrary> IMerged<ICoreLibrary>.Sources => this.GetSources<ICoreLibrary>();

        /// <inheritdoc />
        public bool Equals(ICoreLibrary? other) => _library.Equals(other!);
    }
}
