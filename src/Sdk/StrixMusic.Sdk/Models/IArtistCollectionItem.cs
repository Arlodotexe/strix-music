using StrixMusic.Sdk.Models.Base;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Models.Merged;

namespace StrixMusic.Sdk.Models
{
    /// <inheritdoc cref="IArtistCollectionItemBase"/>
    /// <remarks>This interface should be used in the Sdk.</remarks>
    public interface IArtistCollectionItem : IArtistCollectionItemBase, ISdkMember, IMerged<ICoreArtistCollectionItem>
    {
    }
}