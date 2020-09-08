namespace StrixMusic.Sdk.Interfaces.AbstractUI
{
    /// <summary>
    /// Presents markdown-formatted text to the user.
    /// </summary>
    public interface IAbstractRichTextBlock : IAbstractUIMetadata
    {
        /// <summary>
        /// Markdown-formatted text.
        /// </summary>
        public string RichText { get; }
    }
}
