using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using OwlCore.Extensions;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;

namespace StrixMusic.Sdk.WinUI.Controls
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

        /// <summary>
        /// Dependency property for <see cref="ImageCollection"/>.
        /// </summary>
        public static readonly DependencyProperty ImageCollectionProperty =
            DependencyProperty.Register(nameof(ImageCollection), typeof(IImageCollectionViewModel), typeof(SafeImage), new PropertyMetadata(null, (inst, d) => inst.Cast<SafeImage>().RequestImages().Forget()));

        /// <summary>
        /// The image collection to load and display.
        /// </summary>
        public IImageCollectionViewModel? ImageCollection
        {
            get { return (IImageCollectionViewModel?)GetValue(ImageCollectionProperty); }
            set { SetValue(ImageCollectionProperty, value); }
        }

        /// <inheritdoc/>
        protected override void OnApplyTemplate()
        {
            // Find Parts
            PART_ImageRectangle = GetTemplateChild(nameof(PART_ImageRectangle)) as Rectangle;

            if (PART_ImageRectangle?.Fill is ImageBrush imgBrush)
                PART_ImageBrush = imgBrush;
            else
                ThrowHelper.ThrowInvalidDataException($"{nameof(PART_ImageRectangle)}'s fill must an ImageBrush.");

            _ = RequestImages();
            base.OnApplyTemplate();
        }

        private Rectangle? PART_ImageRectangle { get; set; }

        private ImageBrush? PART_ImageBrush { get; set; }

        private async Task RequestImages()
        {
            if (PART_ImageBrush is null)
                return;

            if (ImageCollection is null)
            {
                PART_ImageBrush.ImageSource = null;
                return;
            }

            var images = await ImageCollection.GetImagesAsync(1, 0).ToListAsync();
            if (images.Count == 0)
            {
                PART_ImageBrush.ImageSource = null;
                return;
            }

            var image = images[0];
            using var stream = await image.OpenStreamAsync();

            var imageSource = new BitmapImage();

            if (image.Height is not null)
                imageSource.DecodePixelHeight = (int)image.Height;

            if (image.Width is not null)
                imageSource.DecodePixelWidth = (int)image.Width;

            if (!stream.CanSeek)
                return;

            await imageSource.SetSourceAsync(stream.AsRandomAccessStream());

            PART_ImageBrush.ImageSource = imageSource;
        }
    }
}
