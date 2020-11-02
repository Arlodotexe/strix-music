using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Sdk.Data
{
    /// <inheritdoc cref="IArtistCollectionItemBase"/>
    /// <remarks>This interface should be used in the Sdk.</remarks>
    public interface IArtistCollectionItem : IArtistCollectionItemBase, ISdkMember<ICoreArtistCollectionItem>
    {
    }
}