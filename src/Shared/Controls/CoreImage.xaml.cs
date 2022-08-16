using System;
using System.IO;
using System.Threading.Tasks;
using StrixMusic.Sdk.CoreModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace StrixMusic.Controls
{
    /// <summary>
    /// Displays the provided instance of <see cref="ICoreImage"/>.
    /// </summary>
    public sealed partial class CoreImage : UserControl
    {
        /// <summary>
        /// Creates a new instance of <see cref="CoreImage"/>.
        /// </summary>
        public CoreImage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// The image source.
        /// </summary>
        public ImageSource? Source => (ImageSource)GetValue(SourceProperty);

        /// <summary>
        /// The backing dependency property for <see cref="Source"/>.
        /// </summary>
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register(nameof(Source), typeof(ImageSource), typeof(CoreImage), new PropertyMetadata(null));

        /// <summary>
        /// The image to display.
        /// </summary>
        public ICoreImage? Image
        {
            get => (ICoreImage)GetValue(ImageProperty);
            set => SetValue(ImageProperty, value);
        }

        /// <summary>
        /// The backing dependency property for <see cref="Image"/>.
        /// </summary>
        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register(nameof(Image), typeof(ICoreImage), typeof(CoreImage), new PropertyMetadata(null, (d, e) => _ = ((CoreImage)d).OnImageChanged(e.OldValue as ICoreImage, e.NewValue as ICoreImage)));

        private async Task OnImageChanged(ICoreImage? oldValue, ICoreImage? newValue)
        {
            if (newValue is null)
                return;

            using var stream = await newValue.OpenStreamAsync();

            if (newValue.MimeType?.Contains("svg") ?? false)
            {
                var imageSource = new SvgImageSource();

                if (newValue.Height is not null)
                    imageSource.RasterizePixelHeight = (double)newValue.Height;

                if (newValue.Width is not null)
                    imageSource.RasterizePixelWidth = (double)newValue.Width;
                
                await imageSource.SetSourceAsync(stream.AsRandomAccessStream());

                SetValue(SourceProperty, imageSource);
            }
            else
            {
                var imageSource = new BitmapImage();
                await imageSource.SetSourceAsync(stream.AsRandomAccessStream());

                SetValue(SourceProperty, imageSource);
            }
        }
    }
}
