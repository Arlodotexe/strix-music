using OwlCore.Extensions;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Controls
{
    /// <summary>
    /// A control to display the Strix icon.
    /// </summary>
    public sealed partial class StrixIcon : UserControl
    {
        /// <summary>
        /// The backing <see cref="DependencyProperty"/> for the <see cref="ShowText"/> property.
        /// </summary>
        public static readonly DependencyProperty ShowTextProperty =
            DependencyProperty.Register(nameof(ShowText), typeof(bool), typeof(StrixIcon), new PropertyMetadata(false));

        /// <summary>
        /// The backing <see cref="DependencyProperty"/> for the <see cref="IsAnimated"/> property.
        /// </summary>
        public static readonly DependencyProperty IsAnimatedProperty =
            DependencyProperty.Register(nameof(IsAnimated), typeof(bool), typeof(StrixIcon), new PropertyMetadata(false, (d, e) => d.Cast<StrixIcon>().OnAnimatedChanged()));

        /// <summary>
        /// Initializes a new instance of the <see cref="StrixIcon"/> class.
        /// </summary>
        public StrixIcon()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the text should show by the icon.
        /// </summary>
        public bool ShowText
        {
            get { return (bool)GetValue(ShowTextProperty); }
            set { SetValue(ShowTextProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the icon should show by animated.
        /// </summary>
        public bool IsAnimated
        {
            get { return (bool)GetValue(IsAnimatedProperty); }
            set { SetValue(IsAnimatedProperty, value); }
        }

        private void OnAnimatedChanged()
        {
            if (IsAnimated)
            {
                Animation.Begin();
            }
            else
            {
                Animation.Stop();
            }
        }
    }
}
