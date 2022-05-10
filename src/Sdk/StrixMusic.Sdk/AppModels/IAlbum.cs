// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using StrixMusic.Sdk.AdapterModels;
using StrixMusic.Sdk.BaseModels;
using StrixMusic.Sdk.CoreModels;

namespace StrixMusic.Sdk.AppModels
{
    /// <summary>
    /// A published album containing one or more tracks, discs, artist, etc.
    /// </summary>
    /// <remarks>Instances of this class may contain data merged from one or more sources.</remarks>
    public interface IAlbum : IAlbumBase, IAlbumCollectionItem, IArtistCollection, ITrackCollection, IImageCollection, IUrlCollection, IGenreCollection, IPlayable, IAppModel, IMerged<ICoreAlbum>, IMerged<ICoreAlbumCollectionItem>
    {
        /// <summary>
        /// A <see cref="IPlayableCollectionGroupBase"/> of items related to this item.
        /// </summary>
        #warning TODO needs a changed event to facilitate merging in new sources with non-null values.
        IPlayableCollectionGroup? RelatedItems { get; }
    }
}
