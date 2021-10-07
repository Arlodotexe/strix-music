using Microsoft.Toolkit.Diagnostics;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Uno.Helpers;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Sdk.ViewModels.Helpers;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;

namespace StrixMusic.Sdk.Uno.Controls
{
    /// <summary>
    /// An control that displays an image, or a backup image if there is none.
    /// </summary>
    /// <remarks>
    /// This control takes an <see cref="IImageCollection"/> and displays only the first, or none if none exist.
    /// </remarks>
    [TemplatePart(Name = nameof(PART_ImageRectangle), Type = typeof(Rectangle))]
    public sealed partial class SafeImage : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SafeImage"/> class.
        /// </summary>
        public SafeImage()
        {
            DefaultStyleKey = typeof(SafeImage);
        }

        /// <inheritdoc/>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Loaded += SafeImage_Loaded;
        }

        private Rectangle? PART_ImageRectangle { get; set; }

        private ImageBrush? PART_ImageBrush { get; set; }

        private void AttachHandlers()
        {
            Unloaded += SafeImage_Unloaded;
            DataContextChanged += SafeImage_DataContextChanged;
        }

        private void DetachHandlers()
        {
            Unloaded -= SafeImage_Unloaded;
            DataContextChanged -= SafeImage_DataContextChanged;
        }

        private async void SafeImage_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= SafeImage_Loaded;

            // Find Parts
            PART_ImageRectangle = GetTemplateChild(nameof(PART_ImageRectangle)) as Rectangle;

            if (PART_ImageRectangle?.Fill is ImageBrush imgBrush)
                PART_ImageBrush = imgBrush;
            else
                ThrowHelper.ThrowInvalidDataException($"{nameof(PART_ImageRectangle)}'s fill must an ImageBrush.");

            await RequestImages();
            AttachHandlers();
        }

        private void SafeImage_Unloaded(object sender, RoutedEventArgs e)
        {
            DetachHandlers();
        }

        private async void SafeImage_DataContextChanged(DependencyObject sender, DataContextChangedEventArgs args)
        {
            await RequestImages();
        }

        private async Task RequestImages()
        {
            if (!(DataContext is IImageCollectionViewModel viewModel))
                return;

            if (PART_ImageBrush is null)
                return;

            var images = await viewModel.GetImagesAsync(1, 0);
            if (images.Count == 0)
                return;

            var image = images[0];

            PART_ImageBrush.ImageSource = new BitmapImage(image.Uri)
            {
                DecodePixelHeight = (int)image.Height,
                DecodePixelWidth = (int)image.Width,
            };
        }
    }
}
