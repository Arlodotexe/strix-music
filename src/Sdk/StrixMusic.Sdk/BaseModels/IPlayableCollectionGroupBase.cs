// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

namespace StrixMusic.Sdk.BaseModels
{
    /// <summary>
    /// Multiple playable collections that are grouped together under a single context.
    /// </summary>
    public interface IPlayableCollectionGroupBase : IPlaylistCollectionBase, ITrackCollectionBase, IAlbumCollectionBase, IArtistCollectionBase, ICollectionItemBase, IPlayableCollectionGroupChildrenBase
    {
    }
}
