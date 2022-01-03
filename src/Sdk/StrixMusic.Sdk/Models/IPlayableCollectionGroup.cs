using System.Diagnostics.CodeAnalysis;
using StrixMusic.Sdk.Models.Base;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Models.Merged;

namespace StrixMusic.Sdk.Models
{
    /// <inheritdoc cref="IPlayableCollectionGroupBase"/>
    /// <remarks>This interface should be implemented by the Sdk.</remarks>
    [SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity", Justification = "Ambiguity is handled")]
    public interface IPlayableCollectionGroup : IPlayableCollectionGroupBase, IPlaylistCollection, ITrackCollection, IAlbumCollection, IArtistCollection, IPlayableCollectionGroupChildren, ISdkMember, IMerged<ICorePlayableCollectionGroup>
    {
    }
}