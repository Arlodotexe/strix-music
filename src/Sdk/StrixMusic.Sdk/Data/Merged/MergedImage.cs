using Microsoft.Toolkit.Diagnostics;
using StrixMusic.Sdk.Data.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StrixMusic.Sdk.Data.Merged
{
    /// <summary>
    /// Merged multiple <see cref="ICoreImage"/> into a single <see cref="IImage"/>
    /// </summary>
    public class MergedImage : IImage, IMerged<ICoreImage>
    {
        private readonly ICoreImage _preferredSource;
        private readonly List<ICoreImage> _sources;

        /// <summary>
        /// Initializes a new instance of the <see cref="MergedImage"/> class.
        /// </summary>
        public MergedImage(IReadOnlyList<ICoreImage> sources)
        {
            Guard.IsNotNull(sources, nameof(sources));
            _sources = sources.ToList();

            _preferredSource = _sources[0];
        }

        /// <inheritdoc/>
        public Uri Uri => _preferredSource.Uri;

        /// <inheritdoc/>
        public double Height => _preferredSource.Height;

        /// <inheritdoc/>
        public double Width => _preferredSource.Width;

        /// <inheritdoc/>
        public IReadOnlyList<ICore> SourceCores => Sources.Select(x => x.SourceCore).ToList();

        /// <inheritdoc/>
        public IReadOnlyList<ICoreImage> Sources => _sources;

        /// <inheritdoc/>
        IReadOnlyList<ICoreImage> ISdkMember<ICoreImage>.Sources => Sources;

        /// <inheritdoc/>
        public void AddSource(ICoreImage itemToMerge) => _sources.Add(itemToMerge);

        /// <inheritdoc />
        public void RemoveSource(ICoreImage itemToRemove) => _sources.Remove(itemToRemove);

        /// <inheritdoc/>
        public bool Equals(ICoreImage other)
        {
            return other?.Uri == Uri;
        }
    }
}
