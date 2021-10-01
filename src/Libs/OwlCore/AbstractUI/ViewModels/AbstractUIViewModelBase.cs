using System;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using OwlCore.AbstractUI.Models;

namespace OwlCore.AbstractUI.ViewModels
{
    /// <summary>
    /// Base view model for all AbstractUI element ViewModels.
    /// </summary>
    public class AbstractUIViewModelBase : ObservableObject, IDisposable
    {
        /// <summary>
        /// Creates a new instance of <see cref="AbstractUIViewModelBase"/>.
        /// </summary>
        /// <param name="model"></param>
        public AbstractUIViewModelBase(AbstractUIBase model)
        {
            if (model.ImagePath != null)
            {
                //TODO: Check for a valid image.
                ImageSourceIsValid = true;
            }

            Model = model;
            AttachEvents();
        }

        private void AttachEvents()
        {
            Model.IconCodeChanged += Model_IconCodeChanged;
            Model.ImagePathChanged += Model_ImagePathChanged;
            Model.SubtitleChanged += Model_SubtitleChanged;
            Model.TitleChanged += Model_TitleChanged;
            Model.TooltipTextChanged += Model_TooltipTextChanged;
        }

        private void DetachEvents()
        {
            Model.IconCodeChanged -= Model_IconCodeChanged;
            Model.ImagePathChanged -= Model_ImagePathChanged;
            Model.SubtitleChanged -= Model_SubtitleChanged;
            Model.TitleChanged -= Model_TitleChanged;
            Model.TooltipTextChanged -= Model_TooltipTextChanged;
        }

        private void Model_TooltipTextChanged(object sender, string? e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(TooltipText)));

        private void Model_TitleChanged(object sender, string? e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(Title)));

        private void Model_SubtitleChanged(object sender, string? e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(Subtitle)));

        private void Model_IconCodeChanged(object sender, string? e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(IconCode)));

        private void Model_ImagePathChanged(object sender, string? e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(ImageSource)));

        /// <summary>
        /// The proxied model used by this class.
        /// </summary>
        public AbstractUIBase Model { get; }

        /// <summary>
        /// An identifier for this item.
        /// </summary>
        public string Id => Model.Id;

        /// <summary>
        /// A title to display for this item.
        /// </summary>
        /// <remarks>Shells are required to support this property on all AbstractUI elements.</remarks>
        public string? Title
        {
            get => Model.Title;
            set => SetProperty(Model.Title, value, Model, (u, n) => Model.Title = n);
        }

        /// <summary>
        /// An optional subtitle to display with the title.
        /// </summary>
        /// <remarks>Shells are required to support this property on all AbstractUI elements.</remarks>
        public string? Subtitle
        {
            get => Model.Subtitle;
            set => SetProperty(Model.Subtitle, value, Model, (u, n) => Model.Subtitle = n);
        }

        /// <summary>
        /// Extended markdown-formatted text to display in an info-focused tooltip.
        /// </summary>
        /// <remarks>Shells are required to support this property on all AbstractUI elements.</remarks>
        public string? TooltipText
        {
            get => Model.TooltipText;
            set => SetProperty(Model.TooltipText, value, Model, (u, n) => Model.TooltipText = n);
        }

        /// <summary>
        /// A hex code representing an icon from the Segoe MDL2 Assets to display with this item (optional).
        /// </summary>
        /// <remarks>Example: <example><c>"\xE10F"</c></example></remarks>
        public string? IconCode
        {
            get => Model.IconCode;
            set => SetProperty(Model.IconCode, value, Model, (u, n) => Model.IconCode = n);
        }

        /// <summary>
        /// An image associated with this item (optional)
        /// </summary>
        public string? ImageSource
        {
            get => Model.ImagePath;
            set => SetProperty(Model.ImagePath, value, Model, (u, n) => Model.ImagePath = n);
        }

        /// <summary>
        /// If true, the <see cref="ImageSource"/> is valid and should display an image.
        /// </summary>
        public bool ImageSourceIsValid { get; set; }

        /// <inheritdoc />
        public virtual void Dispose()
        {
            DetachEvents();
        }
    }
}