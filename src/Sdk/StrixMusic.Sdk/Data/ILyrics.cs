using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Data.Merged;

namespace StrixMusic.Sdk.Data
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