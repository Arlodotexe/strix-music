using System.Collections.Generic;
using System.Threading.Tasks;
using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Data.Merged;

namespace StrixMusic.Sdk.Data
{
    /// <summary>
    /// The query and related data about something the user searched for. 
    /// </summary>
    /// <remarks>This interface should be used in a core.</remarks>
    public interface ISearchQuery : ISearchQueryBase, ISdkMember, IMerged<ICoreSearchQuery>
    {
        /// <summary>
        /// The item(s) that the user selected when searching this query (if applicable).
        /// </summary>
        /// <returns>The relevant items, cast to <see cref="IPlayable"/>.</returns>
        public IAsyncEnumerable<T> GetClickedItems<T>()
            where T : IPlayable, ICoreMember;

        /// <summary>
        /// Registers a <see cref="IPlayable"/> with this search query.
        /// </summary>
        /// <param name="playable"></param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task RegisterClickedItem<T>(T playable)
            where T : IPlayable, ICoreMember;
    }
}