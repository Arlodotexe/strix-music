// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using StrixMusic.Sdk.BaseModels;

namespace StrixMusic.Sdk.CoreModels
{
    /// <summary>
    /// A collection of artibrary songs that the user can edit, rearrange and play back.
    /// </summary>
    /// <remarks>This interface should be implemented by a core.</remarks>
    public interface ICorePlaylist : IPlaylistBase, ICoreTrackCollection, ICorePlaylistCollectionItem, ICoreMember
    {
        /// <summary>
        /// Owner of the playable item.
        /// </summary>
        ICoreUserProfile? Owner { get; }

        /// <summary>
        /// A <see cref="ICorePlayableCollectionGroup"/> of items related to this item.
        /// </summary>
        ICorePlayableCollectionGroup? RelatedItems { get; }
    }
}
