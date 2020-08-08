using System;
using System.Collections.Generic;
using System.Text;
using StrixMusic.CoreInterfaces.Interfaces;

namespace StrixMusic.ViewModels.Mergers
{
    /// <summary>
    /// Helps merge ILibraries.
    /// </summary>
    public class LibraryMerger
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LibraryMerger"/> class.
        /// </summary>
        /// <param name="libraries">The libraries to merge</param>
        public LibraryMerger(params ILibrary[] libraries)
        {

        }
    }
}
