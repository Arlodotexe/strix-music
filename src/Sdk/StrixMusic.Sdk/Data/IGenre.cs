using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Data.Merged;

namespace StrixMusic.Sdk.Data
{
    /// <inheritdoc cref="IGenreBase"/>
    /// <remarks>This interface should be implemented in the Sdk.</remarks>
    public interface IGenre : IGenreBase, ISdkMember, IMerged<ICoreGenre>
    {
    }
}