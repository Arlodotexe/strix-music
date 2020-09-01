using StrixMusic.ViewModels;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shell.Default.Controls
{
    /// <summary>
    /// A Templated <see cref="Control"/> for the root UIElemnt of any shell.
    /// </summary>
    public sealed partial class ShellControl : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShellControl"/> class.
        /// </summary>
        public ShellControl()
        {
            this.DefaultStyleKey = typeof(ShellControl);
        }

        /// <summary>
        /// The <see cref="MainViewModel"/> for the app.
        /// </summary>
        public MainViewModel? ViewModel => DataContext as MainViewModel;
    }
}
