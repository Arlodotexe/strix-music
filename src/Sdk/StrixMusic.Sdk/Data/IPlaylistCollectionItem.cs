using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Data.Merged;

namespace StrixMusic.Sdk.Data
{
    /// <inheritdoc cref="IPlaylistCollectionItemBase"/>
    /// <remarks>This interface should be used in the Sdk.</remarks>
    public interface IPlaylistCollectionItem : IPlaylistCollectionItemBase, ISdkMember, IMerged<ICorePlaylistCollectionItem>
    {
    }
}