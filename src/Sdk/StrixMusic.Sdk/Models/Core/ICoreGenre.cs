using StrixMusic.Sdk.Models.Base;

namespace StrixMusic.Sdk.Models.Core
{
    /// <inheritdoc cref="IGenreBase"/>
    /// <remarks>This interface should be implemented in a core.</remarks>
    public interface ICoreGenre : IGenreBase, ICoreMember
    {
    }
}
