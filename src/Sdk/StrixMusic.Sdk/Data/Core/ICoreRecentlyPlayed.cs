using StrixMusic.Sdk.Data.Base;

namespace StrixMusic.Sdk.Data.Core
{
    /// <inheritdoc cref="IRecentlyPlayedBase"/>
    /// <remarks>This interface should be implemented by a core.</remarks>
    public interface ICoreRecentlyPlayed : IRecentlyPlayedBase, ICorePlayableCollectionGroup, ICoreMember
    {
    }
}
