using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.Extensions;
using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Sdk.Data.Merged
{
    /// <summary>
    /// Merged multiple <see cref="ICoreUrl"/> into a single <see cref="IUrl"/>
    /// </summary>
    public class MergedUrl : IUrl, IMergedMutable<ICoreUrl>
    {
        private readonly ICoreUrl _preferredSource;
        private readonly List<ICoreUrl> _sources;
        private readonly List<ICore> _sourceCores;

        /// <summary>
        /// Initializes a new instance of the <see cref="MergedUrl"/> class.
        /// </summary>
        public MergedUrl(IReadOnlyList<ICoreUrl> sources)
        {
            Guard.IsNotNull(sources, nameof(sources));
            _sources = sources.ToList();
            _sourceCores = _sources.Select(x => x.SourceCore).ToList();

            _preferredSource = _sources[0];
        }

        /// <inheritdoc/>
        public string Label => _preferredSource.Label;

        /// <inheritdoc/>
        public Uri Url => _preferredSource.Url;

        /// <inheritdoc/>
        public UrlType Type => _preferredSource.Type;

        /// <inheritdoc/>
        public IReadOnlyList<ICore> SourceCores => _sourceCores;

        /// <inheritdoc cref="IMerged{T}.Sources"/>
        public IReadOnlyList<ICoreUrl> Sources => _sources;

        /// <inheritdoc/>
        void IMergedMutable<ICoreUrl>.AddSource(ICoreUrl itemToMerge)
        {
            Guard.IsNotNull(itemToMerge, nameof(itemToMerge));

            _sources.Add(itemToMerge);
            _sourceCores.Add(itemToMerge.SourceCore);
        }

        /// <inheritdoc />
        void IMergedMutable<ICoreUrl>.RemoveSource(ICoreUrl itemToRemove)
        {
            Guard.IsNotNull(itemToRemove, nameof(itemToRemove));

            _sources.Remove(itemToRemove);
            _sourceCores.Remove(itemToRemove.SourceCore);
        }

        /// <inheritdoc/>
        public bool Equals(ICoreUrl other)
        {
            return other?.Url == Url && 
                   other?.Type == Type &&
                   other?.Label == Label;
        }

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            await _sources.InParallel(x => x.DisposeAsync().AsTask());
        }
    }
}
