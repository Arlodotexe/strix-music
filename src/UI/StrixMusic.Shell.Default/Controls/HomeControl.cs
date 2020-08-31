using StrixMusic.ViewModels.Bindables;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shell.Default.Controls
{
    public sealed partial class HomeControl : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HomeControl"/> class.
        /// </summary>
        public HomeControl()
        {
            this.DefaultStyleKey = typeof(HomeControl);
        }

        private ObservableLibrary? ViewModel => DataContext as ObservableLibrary;
    }
}
