using StrixMusic.Sdk.Models.Base;

namespace StrixMusic.Sdk.Models.Core
{
    /// <summary>
    /// An item that belongs in an <see cref="ICoreAlbumCollection"/> or <see cref="ICoreAlbum"/>.
    /// </summary>
    /// <remarks>This interface should be used by a core.</remarks>
    public interface ICoreAlbumCollectionItem : IAlbumCollectionItemBase, ICoreMember
    {
    }
}