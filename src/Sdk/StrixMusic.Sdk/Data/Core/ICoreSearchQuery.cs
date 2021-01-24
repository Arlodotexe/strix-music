using StrixMusic.Sdk.Data.Base;

namespace StrixMusic.Sdk.Data.Core
{
    /// <summary>
    /// The query and related data about something the user searched for. 
    /// </summary>
    /// <remarks>This interface should be used in a core.</remarks>
    public interface ICoreSearchQuery : ISearchQueryBase, ICoreMember
    {
    }
}