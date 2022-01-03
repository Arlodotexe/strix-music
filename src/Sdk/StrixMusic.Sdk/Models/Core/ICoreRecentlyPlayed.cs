using StrixMusic.Sdk.Models.Base;

namespace StrixMusic.Sdk.Models.Core
{
    /// <inheritdoc cref="IRecentlyPlayedBase"/>
    /// <remarks>This interface should be implemented by a core.</remarks>
    public interface ICoreRecentlyPlayed : IRecentlyPlayedBase, ICorePlayableCollectionGroup, ICoreMember
    {
    }
}
