using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace LaunchPad.Controls
{
    /// <summary>
    /// A simple popup/overlay that can display arbitrary content.
    /// </summary>
    public sealed partial class ContentPopup
    {
        private static ContentPopup? _current;

        /// <summary>
        /// Creates a new instance of <see cref="ContentPopup"/>.
        /// </summary>
        public ContentPopup()
        {
            InitializeComponent();
            _current = this;
        }

        /// <summary>
        /// Shows the ContentPopup (if registered in the XAML tree)
        /// </summary>
        /// <param name="el">The data to show</param>
        public static void Show(FrameworkElement el)
        {
            Show(el, "");
        }

        /// <summary>
        /// Shows the ContentPopup (if registered in the XAML tree)
        /// </summary>
        /// <param name="el">The data to show</param>
        /// <param name="headerText">The header text to show.</param>
        public static void Show(FrameworkElement el, string headerText)
        {
            if(_current is null)
                throw new InvalidOperationException("No content popup was registered.");

            _current.HeaderText.Text = headerText;
            _current.Presenter.Content = el;
            _current.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Hides the <see cref="ContentPopup"/>.
        /// </summary>
        public static void Hide()
        {
            if (_current is null)
                throw new InvalidOperationException("No content popup was registered.");

            _current.Visibility = Visibility.Collapsed;
            _current.Presenter.Content = null;
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e) => Hide();

        private void Background_OnTapped(object sender, TappedRoutedEventArgs e) => Hide();
    }
}