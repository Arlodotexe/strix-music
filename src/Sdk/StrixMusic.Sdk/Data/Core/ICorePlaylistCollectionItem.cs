using StrixMusic.Sdk.Data.Base;

namespace StrixMusic.Sdk.Data.Core
{
    /// <inheritdoc cref="IPlaylistCollectionItemBase"/>
    /// <remarks>This interface should be used in a core.</remarks>
    public interface ICorePlaylistCollectionItem : IPlaylistCollectionItemBase, ICoreMember
    {
    }
}