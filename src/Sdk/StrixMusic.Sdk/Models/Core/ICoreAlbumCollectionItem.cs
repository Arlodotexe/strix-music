using StrixMusic.Sdk.Models.Base;

namespace StrixMusic.Sdk.Models.Core
{
    /// <inheritdoc cref="IAlbumCollectionItemBase"/>
    /// <remarks>This interface should be used by a core.</remarks>
    public interface ICoreAlbumCollectionItem : IAlbumCollectionItemBase, ICoreMember
    {
    }
}