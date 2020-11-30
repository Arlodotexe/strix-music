namespace OwlCore.AbstractUI
{
    /// <summary>
    /// The base for all AbstractUI objects. Contains abstracted metadata.
    /// </summary>
    public abstract class AbstractUIBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="AbstractUIBase"/>.
        /// </summary>
        /// <param name="id"><inheritdoc cref="Id"/></param>
        protected AbstractUIBase(string id)
        {
            Id = id;
        }

        /// <summary>
        /// An identifier for this item.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// A title to display for this item.
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// An optional subtitle to display with the title.
        /// </summary>
        public string? Subtitle { get; set; }

        /// <summary>
        /// Extended markdown-formatted text to display in an info-focused tooltip.
        /// </summary>
        public string? TooltipText { get; set; }

        /// <summary>
        /// A hex code representing an icon from the Segoe MDL2 Assets to display with this item (optional).
        /// </summary>
        /// <remarks>Example: <example><c>"\xE10F"</c></example></remarks>
        public string? IconCode { get; set; }

        /// <summary>
        /// A local path or url pointing to an image associated with this item (optional).
        /// </summary>
        public string? ImagePath { get; set; }
    }
}
