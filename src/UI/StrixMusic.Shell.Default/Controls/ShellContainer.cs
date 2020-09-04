using StrixMusic.ViewModels;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shell.Default.Controls
{
    /// <summary>
    /// A Templated <see cref="Control"/> for the root container of any shell.
    /// </summary>
    public sealed partial class ShellContainer : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShellContainer"/> class.
        /// </summary>
        public ShellContainer()
        {
            this.DefaultStyleKey = typeof(ShellContainer);
        }

        /// <summary>
        /// The <see cref="MainViewModel"/> for the app.
        /// </summary>
        public MainViewModel? ViewModel => DataContext as MainViewModel;
    }
}
