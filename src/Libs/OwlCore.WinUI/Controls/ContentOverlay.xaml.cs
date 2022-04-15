using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace OwlCore.WinUI.Controls
{
    /// <summary>
    /// A simple popup/overlay that can display arbitrary content.
    /// </summary>
    public sealed partial class ContentOverlay
    {
        /// <summary>
        /// Creates a new instance of <see cref="ContentOverlay"/>.
        /// </summary>
        public ContentOverlay()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Shows the <see cref="ContentOverlay"/>.
        /// </summary>
        /// <param name="el">The data to show</param>
        /// <param name="headerText">The header text to show.</param>
        public void Show(FrameworkElement el, string headerText = "")
        {
            HeaderText.Text = headerText;
            Presenter.Content = el;

            if (Visibility == Visibility.Collapsed)
            {
                Visibility = Visibility.Visible;
                ShowAnimation.Begin();
            }
        }

        /// <summary>
        /// Hides the <see cref="ContentOverlay"/>.
        /// </summary>
        /// <remarks>
        /// Occurs indirectly.
        /// Visibility is collapsed and content is removed after animation.
        /// </remarks>
        public void Hide()
        {
            HideAnimation.Begin();
        }

        private void CompleteHide()
        {
            Visibility = Visibility.Collapsed;
            Presenter.Content = null;
            Closed?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Raised when the content overlay is closed.
        /// </summary>
        public event EventHandler? Closed;

        private void CloseButton_OnClick(object sender, RoutedEventArgs e) => Hide();

        private void Background_OnTapped(object sender, TappedRoutedEventArgs e) => Hide();

        private void HideAnimation_Completed(object sender, object e) => CompleteHide();
    }
}