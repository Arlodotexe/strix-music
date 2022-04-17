// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using OwlCore.Extensions;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;

namespace StrixMusic.Sdk.AdapterModels
{
    /// <summary>
    /// Merged multiple <see cref="ICoreGenre"/> into a single <see cref="IGenre"/>
    /// </summary>
    public class MergedGenre : IGenre, IMergedMutable<ICoreGenre>
    {
        private readonly ICoreGenre _preferredSource;
        private readonly List<ICoreGenre> _sources;
        private readonly List<ICore> _sourceCores;

        /// <summary>
        /// Initializes a new instance of the <see cref="MergedGenre"/> class.
        /// </summary>
        public MergedGenre(IReadOnlyList<ICoreGenre> sources)
        {
            Guard.IsNotNull(sources, nameof(sources));
            _sources = sources.ToList();
            _sourceCores = _sources.Select(x => x.SourceCore).ToList();

            _preferredSource = _sources[0];
        }

        /// <inheritdoc/>
        public string Name => _preferredSource.Name;

        /// <inheritdoc/>
        public IReadOnlyList<ICore> SourceCores => _sourceCores;

        /// <inheritdoc cref="IMerged{T}.Sources"/>
        public IReadOnlyList<ICoreGenre> Sources => _sources;

        /// <inheritdoc/>
        void IMergedMutable<ICoreGenre>.AddSource(ICoreGenre itemToMerge)
        {
            Guard.IsNotNull(itemToMerge, nameof(itemToMerge));

            _sources.Add(itemToMerge);
            _sourceCores.Add(itemToMerge.SourceCore);
        }

        /// <inheritdoc />
        void IMergedMutable<ICoreGenre>.RemoveSource(ICoreGenre itemToRemove)
        {
            Guard.IsNotNull(itemToRemove, nameof(itemToRemove));

            _sources.Remove(itemToRemove);
            _sourceCores.Remove(itemToRemove.SourceCore);
        }

        /// <inheritdoc/>
        public bool Equals(ICoreGenre other)
        {
            return other?.Name == Name;
        }

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            await _sources.InParallel(x => x.DisposeAsync().AsTask());
        }
    }
}
