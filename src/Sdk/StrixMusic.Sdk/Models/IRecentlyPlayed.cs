using StrixMusic.Sdk.Models.Base;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Models.Merged;

namespace StrixMusic.Sdk.Models
{
    /// <inheritdoc cref="IRecentlyPlayedBase"/>
    /// <remarks>This interface should be implemented by the Sdk.</remarks>
    public interface IRecentlyPlayed : IRecentlyPlayedBase, IPlayableCollectionGroup, IPlayable, ISdkMember, IMerged<ICoreRecentlyPlayed>
    {
    }
}