using StrixMusic.Sdk.Models.Base;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Models.Merged;

namespace StrixMusic.Sdk.Models
{
    /// <inheritdoc cref="IAlbumCollectionItemBase"/>
    /// <remarks>This interface should be used by the Sdk</remarks>
    public interface IAlbumCollectionItem : IAlbumCollectionItemBase, IPlayable, ISdkMember, IMerged<ICoreAlbumCollectionItem>
    {
    }
}