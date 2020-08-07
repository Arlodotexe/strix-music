using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;

namespace StrixMusic.Shell.Default.Controls
{
    /// <summary>
    /// A <see cref="ToggleButton"/> that can change content when toggled
    /// </summary>
    public partial class ToggleContentButton : ToggleButton
    {
        /// <summary>
        /// A <see cref="DependencyProperty"/> for the <see cref="CheckedContent"/> property.
        /// </summary>
        public static readonly DependencyProperty CheckContentProperty = DependencyProperty.Register(
            nameof(CheckedContent),
            typeof(object),
            typeof(ToggleContentButton),
            new PropertyMetadata(null));

        /// <summary>
        /// Initializes a new instance of the <see cref="ToggleContentButton"/> class.
        /// </summary>
        public ToggleContentButton()
        {
            this.DefaultStyleKey = typeof(ToggleContentButton);
        }

        /// <summary>
        /// The <see cref="object"/> the <see cref="ToggleContentButton"/> shows when toggled.
        /// </summary>
        public object CheckedContent
        {
            get { return (object)GetValue(CheckContentProperty); }
            set { SetValue(CheckContentProperty, value); }
        }
    }
}
