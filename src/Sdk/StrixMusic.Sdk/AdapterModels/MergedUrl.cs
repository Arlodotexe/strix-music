// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Diagnostics;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;

namespace StrixMusic.Sdk.AdapterModels
{
    /// <summary>
    /// Merged multiple <see cref="ICoreUrl"/> into a single <see cref="IUrl"/>
    /// </summary>
    public class MergedUrl : IUrl, IMergedMutable<ICoreUrl>
    {
        private readonly ICoreUrl _preferredSource;
        private readonly List<ICoreUrl> _sources;

        /// <summary>
        /// Initializes a new instance of the <see cref="MergedUrl"/> class.
        /// </summary>
        public MergedUrl(IReadOnlyList<ICoreUrl> sources)
        {
            Guard.IsNotNull(sources, nameof(sources));
            _sources = sources.ToList();

            _preferredSource = _sources[0];
        }
        
        /// <inheritdoc cref="IMerged.SourcesChanged"/>
        public event EventHandler? SourcesChanged;

        /// <inheritdoc/>
        public string Label => _preferredSource.Label;

        /// <inheritdoc/>
        public Uri Url => _preferredSource.Url;

        /// <inheritdoc/>
        public UrlType Type => _preferredSource.Type;

        /// <inheritdoc cref="IMerged{T}.Sources"/>
        public IReadOnlyList<ICoreUrl> Sources => _sources;

        /// <inheritdoc/>
        public void AddSource(ICoreUrl itemToMerge)
        {
            Guard.IsNotNull(itemToMerge, nameof(itemToMerge));

            _sources.Add(itemToMerge);
            SourcesChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <inheritdoc />
        public void RemoveSource(ICoreUrl itemToRemove)
        {
            Guard.IsNotNull(itemToRemove, nameof(itemToRemove));

            _sources.Remove(itemToRemove);
            SourcesChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <inheritdoc/>
        public bool Equals(ICoreUrl? other)
        {
            return other?.Url == Url && 
                   other?.Type == Type &&
                   other?.Label == Label;
        }
    }
}
