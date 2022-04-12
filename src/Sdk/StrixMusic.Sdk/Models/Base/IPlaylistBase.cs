// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;

namespace StrixMusic.Sdk.Models.Base
{
    /// <summary>
    /// A collection of arbitrary songs that the user can edit, rearrange and play back.
    /// </summary>
    public interface IPlaylistBase : IPlayableCollectionItem, ITrackCollectionBase, IPlaylistCollectionItemBase, IAsyncDisposable
    {
    }
}
