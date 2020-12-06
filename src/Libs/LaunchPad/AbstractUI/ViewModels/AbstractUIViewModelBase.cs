using Microsoft.Toolkit.Mvvm.ComponentModel;
using OwlCore.AbstractUI.Models;

namespace LaunchPad.AbstractUI.ViewModels
{
    /// <summary>
    /// Base view model for all AbstractUI elements.
    /// </summary>
    public class AbstractUIViewModelBase : ObservableObject
    {
        private readonly AbstractUIBase _model;

        /// <summary>
        /// Creates a new instance of <see cref="AbstractUIViewModelBase"/>.
        /// </summary>
        /// <param name="model"></param>
        public AbstractUIViewModelBase(AbstractUIBase model)
        {
            _model = model;
        }

        /// <summary>
        /// An identifier for this item.
        /// </summary>
        public string Id => _model.Id;

        /// <summary>
        /// A title to display for this item.
        /// </summary>
        public string? Title
        {
            get => _model.Title;
            set => SetProperty(_model.Title, value, _model, (u, n) => _model.Title = n);
        }

        /// <summary>
        /// An optional subtitle to display with the title.
        /// </summary>
        public string? Subtitle
        {
            get => _model.Subtitle;
            set => SetProperty(_model.Subtitle, value, _model, (u, n) => _model.Subtitle = n);
        }

        /// <summary>
        /// Extended markdown-formatted text to display in an info-focused tooltip.
        /// </summary>
        public string? TooltipText
        {
            get => _model.TooltipText;
            set => SetProperty(_model.TooltipText, value, _model, (u, n) => _model.TooltipText = n);
        }

        /// <summary>
        /// A hex code representing an icon from the Segoe MDL2 Assets to display with this item (optional).
        /// </summary>
        /// <remarks>Example: <example><c>"\xE10F"</c></example></remarks>
        public string? IconCode
        {
            get => _model.IconCode;
            set => SetProperty(_model.IconCode, value, _model, (u, n) => _model.IconCode = n);
        }

        /// <summary>
        /// A local path or url pointing to an image associated with this item (optional).
        /// </summary>
        public string? ImagePath
        {
            get => _model.ImagePath;
            set => SetProperty(_model.ImagePath, value, _model, (u, n) => _model.ImagePath = n);
        }
    }
}