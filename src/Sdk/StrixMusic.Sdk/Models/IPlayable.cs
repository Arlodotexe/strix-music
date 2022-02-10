using StrixMusic.Sdk.Models.Base;

namespace StrixMusic.Sdk.Models
{
    /// <summary>
    /// Represents an item that can be played.
    /// </summary>
    /// <remarks>Instances of this class may contain data merged from one or more sources.</remarks>
    public interface IPlayable : IPlayableBase, ISdkMember
    {
    } 
}
