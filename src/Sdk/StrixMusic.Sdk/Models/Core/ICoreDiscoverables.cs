using StrixMusic.Sdk.Models.Base;

namespace StrixMusic.Sdk.Models.Core
{
    /// <inheritdoc cref="IDiscoverablesBase"/>
    /// <remarks>This interface should be implemented by a core.</remarks>
    public interface ICoreDiscoverables : IDiscoverablesBase, ICorePlayableCollectionGroup, ICoreMember
    {
    }
}