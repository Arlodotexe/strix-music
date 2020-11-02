using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Sdk.Data
{
    /// <inheritdoc cref="IGenreCollectionBase" />
    /// <remarks>This interface should be implemented by the Sdk.</remarks>
    public interface IGenreCollection : IGenreCollectionBase, ISdkMember<ICoreGenreCollection>
    {
    }
}