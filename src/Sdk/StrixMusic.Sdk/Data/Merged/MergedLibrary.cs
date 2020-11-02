using System.Collections.Generic;
using System.Linq;
using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Extensions.SdkMember;

namespace StrixMusic.Sdk.Data.Merged
{
    /// <summary>
    /// A concrete class that merges multiple <see cref="ILibraryBase"/>.
    /// </summary>
    public class MergedLibrary : MergedPlayableCollectionGroupBase<ICoreLibrary>, ILibrary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MergedLibrary"/> class.
        /// </summary>
        /// <param name="source">The <see cref="ICoreLibrary"/> objects to merge.</param>
        public MergedLibrary(IEnumerable<ICoreLibrary> source)
            : base(source.ToArray())
        {
        }

        /// <inheritdoc />
        IReadOnlyList<ICoreLibrary> ISdkMember<ICoreLibrary>.Sources => this.GetSources<ICoreLibrary>();
    }
}