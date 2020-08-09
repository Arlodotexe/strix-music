using System.Linq;
using StrixMusic.CoreInterfaces.Interfaces;

namespace StrixMusic.ViewModels
{
    public static partial class Mergers
    {
        /// <summary>
        /// Marges multiple <see cref="ILibrary"/> into one.
        /// </summary>
        /// <param name="libraries">The libraries to merge.</param>
        /// <returns><inheritdoc cref="ILibrary"/></returns>
        public static ILibrary MergeLibrary(params ILibrary[] libraries)
        {
            // TODO
            return libraries.First();
        }
    }
}