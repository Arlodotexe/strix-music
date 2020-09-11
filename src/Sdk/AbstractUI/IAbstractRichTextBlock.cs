namespace StrixMusic.Sdk.AbstractUI
{
    /// <summary>
    /// Presents markdown-formatted text to the user.
    /// </summary>
    public interface IAbstractRichTextBlock : IAbstractUIElement
    {
        /// <summary>
        /// Markdown-formatted text.
        /// </summary>
        public string RichText { get; }
    }
}
