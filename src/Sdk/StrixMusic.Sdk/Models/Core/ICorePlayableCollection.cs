using StrixMusic.Sdk.Models.Base;

namespace StrixMusic.Sdk.Models.Core
{
    /// <summary>
    /// A base class for playable collections in a core.
    /// </summary>
    public interface ICorePlayableCollection : IPlayableCollectionBase, ICoreCollection, ICoreMember
    {
    }
}