// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System.Collections.Generic;
using CommunityToolkit.Diagnostics;
using StrixMusic.Sdk.AdapterModels;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Sdk.Extensions;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// A ViewModel for <see cref="ILibrary"/>.
    /// </summary>
    public sealed class LibraryViewModel : PlayableCollectionGroupViewModel, ISdkViewModel, ILibrary
    {
        private readonly ILibrary _library;

        /// <summary>
        /// Creates a new instance of the <see cref="LibraryViewModel"/> class.
        /// </summary>
        /// <param name="library">The <see cref="ILibrary"/> to wrap.</param>
        /// <param name="viewModelRoot">The ViewModel-enabled <see cref="IStrixDataRoot" /> which is responsible for creating this and all parent instances.</param>
        public LibraryViewModel(ILibrary library, IStrixDataRoot viewModelRoot)
            : base(library, viewModelRoot)
        {
            Guard.IsOfType<StrixDataRootViewModel>(viewModelRoot);
            _library = library;
        }

        /// <inheritdoc />
        IReadOnlyList<ICoreLibrary> IMerged<ICoreLibrary>.Sources => this.GetSources<ICoreLibrary>();

        /// <inheritdoc />
        public bool Equals(ICoreLibrary other) => _library.Equals(other);
    }
}
