using StrixMusic.Sdk.Models.Base;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Models.Merged;

namespace StrixMusic.Sdk.Models
{
    /// <summary>
    /// The query and related data about something the user searched for. 
    /// </summary>
    /// <remarks>This interface should be used in a core.</remarks>
    public interface ISearchQuery : ISearchQueryBase, ISdkMember, IMerged<ICoreSearchQuery>
    {
    }
}