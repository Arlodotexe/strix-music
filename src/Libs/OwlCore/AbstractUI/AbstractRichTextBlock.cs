using System;

namespace OwlCore.AbstractUI
{
    /// <summary>
    /// Presents markdown-formatted text to the user.
    /// </summary>
    public abstract class AbstractRichTextBlock : AbstractUIElement
    {
        /// <summary>
        /// Creates a new instance of <see cref="AbstractRichTextBlock"/>
        /// </summary>
        /// <param name="id"><inheritdoc cref="AbstractUIBase.Id"/></param>
        /// <param name="richText"><inheritdoc cref="RichText"/></param>
        protected AbstractRichTextBlock(string id, string richText)
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
        public string RichText { get; }
    }
}
