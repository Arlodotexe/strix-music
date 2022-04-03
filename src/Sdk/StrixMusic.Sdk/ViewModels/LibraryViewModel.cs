// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.Models;
using StrixMusic.Sdk.Models.Base;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Models.Merged;

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
        /// <param name="root">The <see cref="MainViewModel"/> that this or the object that created this originated from.</param>
        /// <param name="library">The <see cref="ILibrary"/> to wrap.</param>
        internal LibraryViewModel(MainViewModel root, ILibrary library)
            : base(root, library)
        {
            _library = library;
        }

        /// <inheritdoc />
        IReadOnlyList<ICoreLibrary> IMerged<ICoreLibrary>.Sources => this.GetSources<ICoreLibrary>();

        /// <inheritdoc />
        public bool Equals(ICoreLibrary other) => _library.Equals(other);

        /// <inheritdoc />
        public override Task InitAsync(CancellationToken cancellationToken = default)
        {
            IsInitialized = true;

            // TODO sync library completely or pull from cache
            return Task.CompletedTask;
        }
    }
}
