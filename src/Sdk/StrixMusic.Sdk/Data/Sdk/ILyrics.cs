namespace StrixMusic.Sdk.Core.Data
{
    /// <inheritdoc cref="ILyricsBase"/>
    /// <remarks>This interface should be implemented by the Sdk.</remarks>
    public interface ILyrics : ILyricsBase, ISdkMember
    {
        /// <summary>
        /// The track that these lyrics belong to.
        /// </summary>
        ITrack Track { get; }
    }
}