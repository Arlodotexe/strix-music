using StrixMusic.Sdk.Models.Base;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Models.Merged;

namespace StrixMusic.Sdk.Models
{
    /// <inheritdoc cref="ILyricsBase"/>
    /// <remarks>This interface should be implemented by the Sdk.</remarks>
    public interface ILyrics : ILyricsBase, ISdkMember, IMerged<ICoreLyrics>
    {
        /// <summary>
        /// The track that these lyrics belong to.
        /// </summary>
        ITrack Track { get; }
    }
}