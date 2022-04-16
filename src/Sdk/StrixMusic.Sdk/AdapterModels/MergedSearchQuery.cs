// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Diagnostics;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Sdk.Models;

namespace StrixMusic.Sdk.AdapterModels
{
    /// <summary>
    /// Aggregates multiple <see cref="ICoreSearchQuery"/> instances into one.
    /// </summary>
    public sealed class MergedSearchQuery : ISearchQuery, IMergedMutable<ICoreSearchQuery>
    {
        private readonly List<ICoreSearchQuery> _sources;
        private readonly List<ICore> _sourceCores;

        /// <summary>
        /// Creates a new instance of <see cref="MergedSearchQuery"/>.
        /// </summary>
        /// <param name="sources">The initial source items merged to form this instance.</param>
        public MergedSearchQuery(IReadOnlyList<ICoreSearchQuery> sources)
        {
            _sources = sources.ToList();
            _sourceCores = sources.Select(x => x.SourceCore).ToList();

            // Make sure all items in the given collection match.
            for (var i = 0; i < sources.Count; i++)
            {
                var item = sources[i];
                var nextItem = sources.ElementAtOrDefault(i + 1);

                if (nextItem == null)
                    continue;

                if (item.Query != nextItem.Query || item.CreatedAt != nextItem.CreatedAt)
                    throw new ArgumentException($"Search queries passed to {nameof(MergedSearchQuery)} do not match");
            }

            // If they all match, we can assign any of them as the value.
            Query = sources[0].Query;
            CreatedAt = sources[0].CreatedAt;
        }

        /// <inheritdoc />
        public string Query { get; }

        /// <inheritdoc />
        public DateTime CreatedAt { get; }

        /// <inheritdoc />
        public bool Equals(ICoreSearchQuery other) => other.Query == Query && other.CreatedAt == CreatedAt;

        /// <inheritdoc />
        public IReadOnlyList<ICoreSearchQuery> Sources => _sources;

        /// <inheritdoc />
        public IReadOnlyList<ICore> SourceCores => _sourceCores;

        /// <inheritdoc />
        void IMergedMutable<ICoreSearchQuery>.AddSource(ICoreSearchQuery itemToMerge)
        {
            Guard.IsNotNull(itemToMerge, nameof(itemToMerge));

            _sources.Add(itemToMerge);
            _sourceCores.Add(itemToMerge.SourceCore);
        }

        /// <inheritdoc />
        void IMergedMutable<ICoreSearchQuery>.RemoveSource(ICoreSearchQuery itemToRemove)
        {
            Guard.IsNotNull(itemToRemove, nameof(itemToRemove));

            _sources.Remove(itemToRemove);
            _sourceCores.Remove(itemToRemove.SourceCore);
        }
    }
}
