// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using StrixMusic.Sdk.BaseModels;

namespace StrixMusic.Sdk.CoreModels
{
    /// <summary>
    /// A musician or creator that has published one or more <see cref="ICoreTrack"/>s and <see cref="ICoreAlbum"/>s.
    /// </summary>
    /// <remarks>This interface should be implemented by a core.</remarks>
    public interface ICoreArtist : IArtistBase, ICoreArtistCollectionItem, ICoreAlbumCollection, ICoreTrackCollection, ICoreGenreCollection, ICoreMember
    {
        /// <summary>
        /// A <see cref="IPlayableCollectionGroupBase"/> of items related to this item.
        /// </summary>
        ICorePlayableCollectionGroup? RelatedItems { get; }
    }
}
