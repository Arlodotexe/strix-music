using System.Collections.Generic;
using System.Linq;
using StrixMusic.Sdk.Models.Base;
using StrixMusic.Sdk.Models.Core;

namespace StrixMusic.Sdk.Models.Merged
{
    /// <summary>
    /// A concrete class that merges multiple <see cref="ILibraryBase"/>.
    /// </summary>
    public sealed class MergedLibrary : MergedPlayableCollectionGroupBase<ICoreLibrary>, ILibrary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MergedLibrary"/> class.
        /// </summary>
        /// <param name="sources">The <see cref="ICoreLibrary"/> objects to merge.</param>
        public MergedLibrary(IEnumerable<ICoreLibrary> sources)
            : base(sources.ToArray())
        {
        }

        /// <inheritdoc cref="IMerged{T}.Sources"/>
        IReadOnlyList<ICoreLibrary> IMerged<ICoreLibrary>.Sources => StoredSources;

        /// <inheritdoc cref="Equals(object?)" />
        public override bool Equals(ICoreLibrary? other)
        {
            return other?.Name == Name;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || (obj is ICoreLibrary other && Equals(other));
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return PreferredSource.GetHashCode();
        }
    }
}