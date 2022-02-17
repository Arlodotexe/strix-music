// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using StrixMusic.Sdk.Models.Base;

namespace StrixMusic.Sdk.Models.Core
{
    /// <summary>
    /// Multiple playable collections that are grouped together under a single context.
    /// </summary>
    /// <remarks>This interface should be implemented by a core.</remarks>
    public interface ICorePlayableCollectionGroup : ICorePlayableCollection, IPlayableCollectionGroupBase, ICorePlaylistCollection, ICoreTrackCollection, ICoreAlbumCollection, ICoreArtistCollection, ICorePlayableCollectionGroupChildren, ICoreMember
    {
    }
}