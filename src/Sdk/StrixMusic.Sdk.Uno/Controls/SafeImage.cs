using StrixMusic.Sdk.Core.Data;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Sdk.Uno.Controls
{
    /// <summary>
    /// An control that displays an image, or a (constant) backup image if there is none.
    /// </summary>
    public sealed partial class SafeImage : Control
    {
        /// <summary>
        /// <see cref="DependencyProperty"/> for the <see cref="CornerRadius"/> property.
        /// </summary>
        public static readonly DependencyProperty CornerRadiiProperty =
            DependencyProperty.Register(
                nameof(CornerRadii),
                typeof(bool),
                typeof(ProgressSlider),
                new PropertyMetadata(true));

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeImage"/> class.
        /// </summary>
        public SafeImage()
        {
            this.DefaultStyleKey = typeof(SafeImage);
        }

        /// <summary>
        /// The <see cref="IImageCollection"/> ViewModel of the control.
        /// </summary>
        public IImageCollection ViewModel => (DataContext as IImageCollection)!;

        /// <summary>
        /// Gets or sets the CornerRadii of the image.
        /// </summary>
        public double CornerRadii
        {
            get => (double)GetValue(CornerRadiiProperty);
            set => SetValue(CornerRadiiProperty, value);
        }
    }
}
