using System.Collections.Generic;
using System.Linq;
using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Extensions.SdkMember;

namespace StrixMusic.Sdk.Data.Merged
{
    /// <summary>
    /// A concrete class that merges multiple <see cref="ICoreSearchResults"/>.
    /// </summary>
    public class MergedSearchResults : MergedPlayableCollectionGroupBase<ICoreSearchResults>, ISearchResults
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MergedSearchResults"/> class.
        /// </summary>
        /// <param name="searchResults">The search results to merge.</param>
        public MergedSearchResults(IEnumerable<ICoreSearchResults> searchResults)
            : base(searchResults.ToArray())
        {
        }

        /// <inheritdoc />
        IReadOnlyList<ICoreSearchResults> ISdkMember<ICoreSearchResults>.Sources => this.GetSources<ICoreSearchResults>();
    }
}