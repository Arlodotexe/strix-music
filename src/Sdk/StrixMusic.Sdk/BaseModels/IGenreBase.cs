// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

namespace StrixMusic.Sdk.BaseModels
{
    /// <summary>
    /// Holds details about a genre.
    /// </summary>
    public interface IGenreBase : ICollectionItemBase
    {
        /// <summary>
        /// The name of the genre.
        /// </summary>
        public string Name { get; }
    }
}
