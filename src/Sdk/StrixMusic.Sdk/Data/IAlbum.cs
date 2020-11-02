using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Sdk.Data
{
    /// <inheritdoc cref="IAlbumBase"/>
    /// <remarks>This interface should be implemented by the Sdk.</remarks>
    [SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity", Justification = "Ambiguity is handled")]
    public interface IAlbum : IAlbumBase, IAlbumCollectionItem, ITrackCollection, IImageCollection, ISdkMember<ICoreAlbum>
    {
        /// <summary>
        /// The source albums that were merged into this <see cref="IAlbum"/>.
        /// </summary>
        IReadOnlyList<ICoreAlbum> SourceAlbums { get; }

        /// <summary>
        /// The artist that created this album.
        /// </summary>
        IArtist Artist { get; }

        /// <summary>
        /// A <see cref="IPlayableCollectionGroupBase"/> of items related to this item.
        /// </summary>
        IPlayableCollectionGroup? RelatedItems { get; }
    }
}