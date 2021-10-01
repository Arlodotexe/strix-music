using System;

namespace OwlCore.AbstractUI.Models
{
    /// <summary>
    /// Represents a UI element containing rich text, such as markdown, bbcode, etc.
    /// </summary>
    public class AbstractRichTextBlock : AbstractUIElement
    {
        /// <summary>
        /// Creates a new instance of <see cref="AbstractRichTextBlock"/>
        /// </summary>
        /// <param name="id"><inheritdoc cref="AbstractUIBase.Id"/></param>
        /// <param name="richText"><inheritdoc cref="RichText"/></param>
        public AbstractRichTextBlock(string id, string richText)
            : base(id)
        {
            if (string.IsNullOrEmpty(richText))
            {
                throw new ArgumentException("Value must not be null or empty.", nameof(richText));
            }

            RichText = richText;
        }

        /// <summary>
        /// Markdown-formatted text.
        /// </summary>
        public string RichText { get; set; }
    }
}
