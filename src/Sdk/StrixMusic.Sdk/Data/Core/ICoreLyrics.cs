using StrixMusic.Sdk.Data.Base;

namespace StrixMusic.Sdk.Data.Core
{
    /// <inheritdoc cref="ILyricsBase"/>
    /// <remarks>This interface should be implemented by a core.</remarks>
    public interface ICoreLyrics : ILyricsBase, ICoreMember
    {
        /// <summary>
        /// The track that these lyrics belong to.
        /// </summary>
        ICoreTrack Track { get; }
    }
}
