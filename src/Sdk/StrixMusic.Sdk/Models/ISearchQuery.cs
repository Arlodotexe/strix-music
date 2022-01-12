using StrixMusic.Sdk.Models.Base;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Models.Merged;

namespace StrixMusic.Sdk.Models
{
    /// <summary>
    /// The query and related data about something the user searched for. 
    /// </summary>
    /// <remarks>Instances of this class may contain data merged from one or more sources.</remarks>
    public interface ISearchQuery : ISearchQueryBase, ISdkMember, IMerged<ICoreSearchQuery>
    {
    }
}