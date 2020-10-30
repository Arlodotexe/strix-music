using Microsoft.Toolkit.Diagnostics;
using StrixMusic.Sdk.Core.Data;
using StrixMusic.Sdk.Uno.Helpers;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;

namespace StrixMusic.Sdk.Uno.Controls
{
    /// <summary>
    /// An control that displays an image, or a (constant) backup image if there is none.
    /// </summary>
    [TemplatePart(Name = nameof(PART_ImageRectangle), Type = typeof(Rectangle))]
    [TemplatePart(Name = nameof(PART_Fallback), Type = typeof(Image))]
    public sealed partial class SafeImage : Control
    {
        /*
        /// <summary>
        /// <see cref="DependencyProperty"/> for the <see cref="CornerRadius"/> property.
        /// </summary>
        public static readonly DependencyProperty CornerRadiiProperty =
            DependencyProperty.Register(
                nameof(CornerRadii),
                typeof(bool),
                typeof(SafeImage),
                new PropertyMetadata(true));
        */

        /// <summary>
        /// <see cref="DependencyProperty"/> for the <see cref="SourceCore"/> property.
        /// </summary>
        public static readonly DependencyProperty SourceCoreProperty =
            DependencyProperty.Register(
                nameof(SourceCore),
                typeof(ICore),
                typeof(SafeImage),
                new PropertyMetadata(null));

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeImage"/> class.
        /// </summary>
        public SafeImage()
        {
            this.DefaultStyleKey = typeof(SafeImage);
        }

        /// <inheritdoc/>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Loaded += SafeImage_Loaded;
        }

        /// <summary>
        /// The <see cref="IImage"/> ViewModel of the control.
        /// </summary>
        public IImage? ViewModel => DataContext as IImage;

        /*
        /// <summary>
        /// Gets or sets the CornerRadii of the image.
        /// </summary>
        public double CornerRadii
        {
            get => (double)GetValue(CornerRadiiProperty);
            set => SetValue(CornerRadiiProperty, value);
        }
        */

        /// <summary>
        /// Gets or sets the CornerRadii of the image.
        /// </summary>
        public ICore? SourceCore
        {
            get => (ICore)GetValue(SourceCoreProperty);
            set => SetValue(SourceCoreProperty, value);
        }

        private Rectangle? PART_ImageRectangle { get; set; }

        private ImageBrush? PART_ImageBrush { get; set; }

        private Image? PART_Fallback { get; set; }

        private SvgImageSource? PART_SvgBrush { get; set; }

        private void AttachHandlers()
        {
            Unloaded += SafeImage_Unloaded;

            Guard.IsNotNull(PART_ImageBrush, nameof(PART_ImageBrush));
            PART_ImageBrush!.ImageFailed += SafeImage_ImageFailed;
        }

        private void DetachHandlers()
        {
            Loaded -= SafeImage_Loaded;

            Guard.IsNotNull(PART_ImageBrush, nameof(PART_ImageBrush));
            PART_ImageBrush!.ImageFailed -= SafeImage_ImageFailed;
        }

        private void SafeImage_Loaded(object sender, RoutedEventArgs e)
        {
            // Find Parts
            PART_ImageRectangle = VisualTreeHelpers.GetDataTemplateChild<Rectangle>(this, nameof(PART_ImageRectangle));
            PART_Fallback = VisualTreeHelpers.GetDataTemplateChild<Image>(this, nameof(PART_Fallback));

            Brush brush = PART_ImageRectangle!.Fill;
            if (brush is ImageBrush imgBrush)
            {
                PART_ImageBrush = imgBrush;
            }
            else
            {
                ThrowHelper.ThrowInvalidDataException(string.Format("{0}'s fill must an ImageBrush.", nameof(PART_ImageRectangle)));
            }

            ImageSource source = PART_Fallback!.Source;
            if (source is SvgImageSource svgBrush)
            {
                PART_SvgBrush = svgBrush;
            }
            else
            {
                ThrowHelper.ThrowInvalidDataException(string.Format("{0}'s source must an SvgImageSource.", nameof(PART_Fallback)));
            }

            Setup();

            if (ViewModel == null)
            {
                GoToFallback();
                return; // No need to attach handlers when null
            }

            AttachHandlers();
        }

        private void SafeImage_Unloaded(object sender, RoutedEventArgs e)
        {
            DetachHandlers();
        }

        private void SafeImage_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            GoToFallback();
        }

        private void Setup()
        {
            PART_Fallback!.Visibility = Visibility.Collapsed;
            if (SourceCore != null)
            {
                PART_SvgBrush!.UriSource = SourceCore!.CoreConfig.LogoSvgUrl;
            }
            PART_ImageRectangle!.Visibility = Visibility.Visible;
        }

        private void GoToFallback()
        {
            PART_Fallback!.Visibility = Visibility.Visible;
            PART_ImageRectangle!.Visibility = Visibility.Collapsed;
        }
    }
}
