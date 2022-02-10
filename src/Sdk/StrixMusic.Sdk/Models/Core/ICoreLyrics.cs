using StrixMusic.Sdk.Models.Base;

namespace StrixMusic.Sdk.Models.Core
{
    /// <summary>
    /// Contains the lyrics to a track.
    /// </summary>
    /// <remarks>This interface should be implemented by a core.</remarks>
    public interface ICoreLyrics : ILyricsBase, ICoreMember
    {
        /// <summary>
        /// The track that these lyrics belong to.
        /// </summary>
        ICoreTrack Track { get; }
    }
}
