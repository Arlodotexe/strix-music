using StrixMusic.Sdk.Models.Base;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Models.Merged;

namespace StrixMusic.Sdk.Models
{
    /// <summary>
    /// An item that belongs in an <see cref="IArtistCollection"/>.
    /// </summary>
    /// <remarks>Instances of this class may contain data merged from one or more sources.</remarks>
    public interface IArtistCollectionItem : IArtistCollectionItemBase, IPlayable, ISdkMember, IMerged<ICoreArtistCollectionItem>
    {
    }
}