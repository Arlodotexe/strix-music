using StrixMusic.Sdk.Data.Base;

namespace StrixMusic.Sdk.Data.Core
{
    /// <summary>
    /// A base class for playable collections in a core.
    /// </summary>
    public interface ICorePlayableCollection : IPlayableCollectionBase, ICoreCollection, ICoreMember
    {
    }
}