// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using StrixMusic.Sdk.CoreModels;

namespace StrixMusic.Sdk.AdapterModels
{
    /// <summary>
    /// Provides accessors for modifying the sources in a <see cref="IMerged{T}"/> class.
    /// </summary>
    /// <typeparam name="T">The type that makes up this merged item.</typeparam>
    internal interface IMergedMutable<T>
        where T : ICoreModel
    {
        /// <summary>
        /// Adds a new source to this merged item.
        /// </summary>
        /// <param name="itemToMerge">The source to remove.</param>
        public void AddSource(T itemToMerge);

        /// <summary>
        /// Removes a source from the merged collection.
        /// </summary>
        /// <param name="itemToRemove">The source to remove.</param>
        public void RemoveSource(T itemToRemove);
    }
}
