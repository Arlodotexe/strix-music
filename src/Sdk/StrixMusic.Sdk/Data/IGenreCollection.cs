using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Data.Merged;

namespace StrixMusic.Sdk.Data
{
    /// <inheritdoc cref="IGenreCollectionBase" />
    /// <remarks>This interface should be implemented by the Sdk.</remarks>
    public interface IGenreCollection : IGenreCollectionBase, ISdkMember, IMerged<ICoreGenreCollection>
    {
    }
}