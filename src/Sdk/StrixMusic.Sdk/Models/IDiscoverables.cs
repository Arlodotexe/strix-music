using StrixMusic.Sdk.Models.Base;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Models.Merged;

namespace StrixMusic.Sdk.Models
{
    /// <summary>
    /// Used to browse and discover new music.
    /// </summary>
    /// <remarks>Instances of this class may contain data merged from one or more sources.</remarks>
    public interface IDiscoverables : IDiscoverablesBase, IPlayableCollectionGroup, ISdkMember, IMerged<ICoreDiscoverables>
    {
    }
}