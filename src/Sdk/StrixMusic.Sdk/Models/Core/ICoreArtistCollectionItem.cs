using StrixMusic.Sdk.Models.Base;

namespace StrixMusic.Sdk.Models.Core
{
    /// <inheritdoc cref="IArtistCollectionItemBase"/>
    /// <remarks>This interface should be used in a core.</remarks>
    public interface ICoreArtistCollectionItem : IArtistCollectionItemBase, ICoreMember
    {
    }
}