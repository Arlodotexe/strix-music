﻿// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using StrixMusic.Sdk.BaseModels;

namespace StrixMusic.Sdk.CoreModels
{
    /// <summary>
    /// An item that belongs in an <see cref="ICoreAlbumCollection"/> or <see cref="ICoreAlbum"/>.
    /// </summary>
    /// <remarks>This interface should be used by a core.</remarks>
    public interface ICoreAlbumCollectionItem : IAlbumCollectionItemBase, ICoreMember
    {
    }
}
