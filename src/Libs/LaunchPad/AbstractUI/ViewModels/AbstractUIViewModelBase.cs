using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using OwlCore.AbstractUI.Models;
using OwlCore.Extensions;

namespace LaunchPad.AbstractUI.ViewModels
{
    /// <summary>
    /// Base view model for all AbstractUI elements.
    /// </summary>
    public class AbstractUIViewModelBase : ObservableObject
    {
        private ImageSource _imageSource;

        /// <summary>
        /// Creates a new instance of <see cref="AbstractUIViewModelBase"/>.
        /// </summary>
        /// <param name="model"></param>
        public AbstractUIViewModelBase(AbstractUIBase model)
        {
            Model = model;
            _imageSource = SetupImageSource(model);
        }

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
        public ImageSource ImageSource
        {
            get => _imageSource;
            set => SetProperty(_imageSource, value, _imageSource, (u, n) => _imageSource = n);
        }

        [Pure]
        private ImageSource SetupImageSource(AbstractUIBase model)
        {
            if (Uri.TryCreate(model.ImagePath ?? string.Empty, UriKind.RelativeOrAbsolute, out Uri uri) && string.IsNullOrWhiteSpace(model.ImagePath))
            {
                // If there's no image set, create a 1x1 transparent image as a placeholder.
                // This ensures that bindings won't fail because of an unexpected null value.
                return CreateEmptyImage();
            }

            var ext = Path.GetExtension(model.ImagePath);

            if (ext == ".svg")
            {
                return new SvgImageSource(uri);
            }

            return new BitmapImage(uri)
            {
                DecodePixelType = DecodePixelType.Logical
            };
        }

        /// <summary>
        /// Creates an image with only a 1x1 transparent pixel.
        /// </summary>
        /// <returns>The <see cref="ImageSource"/> of the created image.</returns>
        [Pure]
        private ImageSource CreateEmptyImage()
        {
            var bytes = Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mNkYAAAAAYAAjCB0C8AAAAASUVORK5CYII=");
            var buffer = bytes.AsBuffer();
            var stream = buffer.AsStream();
            var randomStream = stream.AsRandomAccessStream();
            randomStream.Seek(0);

            var image = new BitmapImage();
            image.SetSourceAsync(randomStream).AsTask().RunInBackground();

            return image;
        }
    }
}