using System.Linq;
using StrixMusic.CoreInterfaces.Interfaces;
using StrixMusic.ViewModels.Bindables;

namespace StrixMusic.ViewModels
{
    public static partial class Mergers
    {
        /// <summary>
        /// Marges multiple <see cref="IRecentlyPlayed"/> into one.
        /// </summary>
        /// <param name="recentlyPlayed">The recently played objects to merge.</param>
        /// <returns><inheritdoc cref="ILibrary"/></returns>
        public static IRecentlyPlayed MergeRecentlyPlayed(params IRecentlyPlayed[] recentlyPlayed)
        {
            // TODO
            return recentlyPlayed.First();
        }
    }
}