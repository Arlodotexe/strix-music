using System;
using OwlCore.Remoting;
using OwlCore.Remoting;

namespace OwlCore.AbstractUI.Models
{
    /// <summary>
    /// The base for all AbstractUI objects. Contains abstracted metadata.
    /// </summary>
    [RemoteOptions(RemotingDirection.Bidirectional)]
    public abstract class AbstractUIBase
    {
        private string? _title;
        private string? _subtitle;
        private string? _tooltipText;
        private string? _iconCode;
        private string? _imagePath;

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
        [RemoteProperty]
        public string? Title
        {
            get => _title;
            set
            {
                _title = value;
                TitleChanged?.Invoke(this, _title);
            }
        }

        /// <summary>
        /// An optional subtitle to display with the title.
        /// </summary>
        [RemoteProperty]
        public string? Subtitle
        {
            get => _subtitle;
            set
            {
                _subtitle = value;
                SubtitleChanged?.Invoke(this, _subtitle);
            }
        }

        /// <summary>
        /// Extended markdown-formatted text to display in an info-focused tooltip.
        /// </summary>
        [RemoteProperty]
        public string? TooltipText
        {
            get => _tooltipText;
            set
            {
                _tooltipText = value;
                TooltipTextChanged?.Invoke(this, _tooltipText);
            }
        }

        /// <summary>
        /// A hex code representing an icon from the Segoe MDL2 Assets to display with this item (optional).
        /// </summary>
        /// <remarks>Example: <example><c>"\xE10F"</c></example></remarks>
        [RemoteProperty]
        public string? IconCode
        {
            get => _iconCode;
            set
            {
                _iconCode = value;
                IconCodeChanged?.Invoke(this, _iconCode);
            }
        }

        /// <summary>
        /// A local path or url pointing to an image associated with this item (optional).
        /// </summary>
        [RemoteProperty]
        public string? ImagePath
        {
            get => _imagePath;
            set
            {
                _imagePath = value;
                ImagePathChanged?.Invoke(this, _imagePath);
            }
        }

        /// <summary>
        /// Raised when <see cref="Title"/> is changed.
        /// </summary>
        public event EventHandler<string?>? TitleChanged;

        /// <summary>
        /// Raised when <see cref="Subtitle"/> is changed.
        /// </summary>
        public event EventHandler<string?>? SubtitleChanged;

        /// <summary>
        /// Raised when <see cref="TooltipText"/> is changed.
        /// </summary>
        public event EventHandler<string?>? TooltipTextChanged;

        /// <summary>
        /// Raised when <see cref="IconCode"/> is changed.
        /// </summary>
        public event EventHandler<string?>? IconCodeChanged;

        /// <summary>
        /// Raised when <see cref="ImagePath"/> is changed.
        /// </summary>
        public event EventHandler<string?>? ImagePathChanged;
    }
}
