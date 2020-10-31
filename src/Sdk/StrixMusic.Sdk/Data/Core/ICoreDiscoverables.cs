using StrixMusic.Sdk.Data.Base;

namespace StrixMusic.Sdk.Data.Core
{
    /// <inheritdoc cref="IDiscoverablesBase"/>
    /// <remarks>This interface should be implemented by a core.</remarks>
    public interface ICoreDiscoverables : IDiscoverablesBase, ICorePlayableCollectionGroup, ICoreMember
    {
    }
}