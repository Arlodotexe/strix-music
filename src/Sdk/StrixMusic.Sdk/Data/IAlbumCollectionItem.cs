using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Sdk.Data
{
    /// <inheritdoc cref="IAlbumCollectionItemBase"/>
    /// <remarks>This interface should be used by the Sdk</remarks>
    public interface IAlbumCollectionItem : IAlbumCollectionItemBase, ISdkMember<ICoreAlbumCollectionItem>
    {
    }
}