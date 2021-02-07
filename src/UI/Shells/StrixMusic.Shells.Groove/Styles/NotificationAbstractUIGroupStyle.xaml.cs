using LaunchPad.AbstractUI.Controls;
using LaunchPad.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace StrixMusic.Shells.Groove.Styles
{
    /// <summary>
    /// A <see cref="ResourceDictionary"/> containing the default style for the <see cref="AbstractUIGroupPresenter"/>.
    /// </summary>
    public sealed partial class NotificationAbstractUIGroupStyle : ResourceDictionary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationAbstractUIGroupStyle"/> class.
        /// </summary>
        public NotificationAbstractUIGroupStyle()
        {   
            this.InitializeComponent();
        }

        private void MarqueeTextControl_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            ((MarqueeTextBlock)sender).IsStopped = false;
        }
    }
}
