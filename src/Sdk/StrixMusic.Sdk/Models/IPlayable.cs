using StrixMusic.Sdk.Models.Base;

namespace StrixMusic.Sdk.Models
{
    /// <inheritdoc cref="IPlayableBase"/>
    /// <remarks>This interface should be implemented by the Sdk.</remarks>
    public interface IPlayable : IPlayableBase, ISdkMember
    {
    } 
}
