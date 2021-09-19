using OwlCore.Uno.AbstractUI.Controls;
using OwlCore.Uno.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace StrixMusic.Shells.Groove.Styles.Shells
{
    /// <summary>
    /// A <see cref="ResourceDictionary"/> containing the default style for the <see cref="AbstractUICollectionPresenter"/>.
    /// </summary>
    public sealed partial class NotificationAbstractUICollectionStyle : ResourceDictionary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationAbstractUICollectionStyle"/> class.
        /// </summary>
        public NotificationAbstractUICollectionStyle()
        {   
            this.InitializeComponent();
        }

        private void MarqueeTextControl_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            ((MarqueeTextBlock)sender).IsStopped = false;
        }
    }
}
