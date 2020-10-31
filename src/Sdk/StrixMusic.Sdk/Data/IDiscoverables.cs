using StrixMusic.Sdk.Data.Base;

namespace StrixMusic.Sdk.Data
{
    /// <inheritdoc cref="IDiscoverablesBase"/>
    /// <remarks>This interface should be implemented by the Sdk.</remarks>
    public interface IDiscoverables : IDiscoverablesBase, IPlayableCollectionGroup, ISdkMember
    {
    }
}