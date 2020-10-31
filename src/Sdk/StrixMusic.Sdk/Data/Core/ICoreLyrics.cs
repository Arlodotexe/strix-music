namespace StrixMusic.Sdk.Core.Data
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
