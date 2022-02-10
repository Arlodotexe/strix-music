using StrixMusic.Sdk.Models.Base;

namespace StrixMusic.Sdk.Models.Core
{
    /// <summary>
    /// Holds details about a genre.
    /// </summary>
    /// <remarks>This interface should be implemented in a core.</remarks>
    public interface ICoreGenre : IGenreBase, ICoreMember
    {
    }
}
