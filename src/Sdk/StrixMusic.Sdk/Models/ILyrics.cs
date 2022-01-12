using StrixMusic.Sdk.Models.Base;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Models.Merged;

namespace StrixMusic.Sdk.Models
{
    /// <summary>
    /// Contains the lyrics to a track.
    /// </summary>
    /// <remarks>Instances of this class may contain data merged from one or more sources.</remarks>
    public interface ILyrics : ILyricsBase, ISdkMember, IMerged<ICoreLyrics>
    {
        /// <summary>
        /// The track that these lyrics belong to.
        /// </summary>
        ITrack Track { get; }
    }
}