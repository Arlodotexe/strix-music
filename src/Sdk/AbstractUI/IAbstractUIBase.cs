namespace StrixMusic.Sdk.AbstractUI
{
    /// <summary>
    /// The base for all AbstractUI objects. Contains abstracted metadata.
    /// </summary>
    public interface IAbstractUIBase
    {
        /// <summary>
        /// An identifier for this item.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// A title to display for this item.
        /// </summary>
        public string? Title { get; }

        /// <summary>
        /// An optional subtitle to display with the title.
        /// </summary>
        public string? Subtitle { get; }

        /// <summary>
        /// Extended markdown-formatted text to display in an info-focused tooltip.
        /// </summary>
        public string? TooltipText { get; }

        /// <summary>
        /// A hex code representing an icon from the Segoe MDL2 Assets to display with this item (optional).
        /// </summary>
        /// <remarks>Example: <example><c>"\xE10F"</c></example></remarks>
        public string? IconCode { get; }

        /// <summary>
        /// A local path or url pointing to an image associated with this item (optional).
        /// </summary>
        public string? ImagePath { get; }
    }
}
