using StrixMusic.Sdk.Models.Base;

namespace StrixMusic.Sdk.Models.Core
{
    /// <summary>
    /// An item that belongs in an <see cref="ICoreArtistCollection"/>.
    /// </summary>
    /// <remarks>This interface should be used in a core.</remarks>
    public interface ICoreArtistCollectionItem : IArtistCollectionItemBase, ICoreMember
    {
    }
}