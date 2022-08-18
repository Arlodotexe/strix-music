// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using StrixMusic.Sdk.AppModels;

namespace StrixMusic.Sdk.BaseModels
{
    /// <summary>
    /// A musician or creator that has published one or more <see cref="ITrack"/>s and <see cref="IAlbum"/>s.
    /// </summary>
    public interface IArtistBase : IPlayableCollectionItem, IArtistCollectionItemBase, IAlbumCollectionBase, ITrackCollectionBase, IGenreCollectionBase
    {
    }
}
