using StrixMusic.Sdk.Models.Base;

namespace StrixMusic.Sdk.Models.Core
{
    /// <summary>
    /// Used to browse and discover new music.
    /// </summary>
    /// <remarks>This interface should be implemented by a core.</remarks>
    public interface ICoreDiscoverables : IDiscoverablesBase, ICorePlayableCollectionGroup, ICoreMember
    {
    }
}