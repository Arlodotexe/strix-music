using StrixMusic.Sdk.Models.Base;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Models.Merged;

namespace StrixMusic.Sdk.Models
{
    /// <inheritdoc cref="IPlaylistCollectionItemBase"/>
    /// <remarks>This interface should be used in the Sdk.</remarks>
    public interface IPlaylistCollectionItem : IPlaylistCollectionItemBase, IPlayable, ISdkMember, IMerged<ICorePlaylistCollectionItem>
    {
    }
}