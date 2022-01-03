using StrixMusic.Sdk.Models.Base;

namespace StrixMusic.Sdk.Models.Core
{
    /// <inheritdoc cref="IPlaylistCollectionItemBase"/>
    /// <remarks>This interface should be used in a core.</remarks>
    public interface ICorePlaylistCollectionItem : IPlaylistCollectionItemBase, ICoreMember
    {
    }
}