using StrixMusic.Sdk.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.Groove.Controls.Pages
{
    /// <summary>
    /// A <see cref="Control"/> to display a <see cref="LibraryViewModel"/> on a page.
    /// </summary>
    public partial class GrooveHomePage : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GrooveHomePage"/> class.
        /// </summary>
        public GrooveHomePage()
        {
            DefaultStyleKey = typeof(GrooveHomePage);
        }

        /// <summary>
        /// The backing property for <see cref="Library"/>.
        /// </summary>
        public static readonly DependencyProperty LibraryProperty =
            DependencyProperty.Register(nameof(Library), typeof(LibraryViewModel), typeof(GrooveHomePage), new PropertyMetadata(null, (d, e) => ((GrooveHomePage)d).OnLibraryChanged()));

        /// <summary>
        /// The library displayed in this view.
        /// </summary>
        public LibraryViewModel? Library
        {
            get { return (LibraryViewModel)GetValue(LibraryProperty); }
            set { SetValue(LibraryProperty, value); }
        }

        private void OnLibraryChanged()
        {
        }
    }
}
