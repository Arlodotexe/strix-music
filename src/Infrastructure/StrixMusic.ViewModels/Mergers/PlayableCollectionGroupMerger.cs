using System.Collections.Generic;
using System.Linq;
using StrixMusic.CoreInterfaces.Interfaces;

namespace StrixMusic.ViewModels
{
    public static partial class Mergers
    {
        /// <summary>
        /// Marges multiple <see cref="IPlayableCollectionGroup"/> into one.
        /// </summary>
        /// <param name="collectionGroups">The <see cref="IPlayableCollectionGroup"/>s to merge.</param>
        /// <returns><inheritdoc cref="IPlayableCollectionGroup"/></returns>
        public static IAsyncEnumerable<IPlayableCollectionGroup>? MergePlayableCollectionGroups(params IAsyncEnumerable<IPlayableCollectionGroup>?[] collectionGroups)
        {
            // TODO
            return collectionGroups?.FirstOrDefault();
        }

        /// <summary>
        /// Marges multiple <see cref="IPlayableCollectionGroup"/> into one.
        /// </summary>
        /// <param name="collectionGroups">The libraries to merge.</param>
        /// <returns><inheritdoc cref="IPlayableCollectionGroup"/></returns>
        public static IPlayableCollectionGroup? MergePlayableCollectionGroups(params IPlayableCollectionGroup?[] collectionGroups)
        {
            // TODO
            return collectionGroups?.FirstOrDefault();
        }
    }
}
