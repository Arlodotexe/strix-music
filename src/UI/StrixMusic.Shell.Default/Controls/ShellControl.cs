using StrixMusix.ViewModels;
using Windows.UI.Xaml.Controls;

namespace Strix_Music.Shell.Default.Controls
{
    public sealed partial class ShellControl : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShellControl"/> class.
        /// </summary>
        public ShellControl()
        {
            this.DefaultStyleKey = typeof(ShellControl);
        }

        private MainViewModel? ViewModel => DataContext as MainViewModel;
    }
}
