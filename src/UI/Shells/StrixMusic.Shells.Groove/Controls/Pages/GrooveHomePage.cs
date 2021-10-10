using Microsoft.Toolkit.Diagnostics;
using OwlCore.Extensions;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.Groove.ViewModels.Pages;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.Groove.Controls.Pages
{
    public partial class GrooveHomePage : Control
    {
        /// <summary>
        /// The backing dependency property for <see cref="ViewModel"/>.
        /// </summary>
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(nameof(ViewModel), typeof(GrooveHomePageViewModel), typeof(GrooveHomePage), new PropertyMetadata(null));

        /// <summary>
        /// The backing dependency property for <see cref="Library"/>.
        /// </summary>
        public static readonly DependencyProperty LibraryProperty =
            DependencyProperty.Register(nameof(Library), typeof(AlbumViewModel), typeof(GrooveAlbumPage), new PropertyMetadata(null, (d, e) => d.Cast<GrooveHomePage>().OnLibraryChanged()));

        public GrooveHomePage()
        {
            this.DefaultStyleKey = typeof(GrooveHomePage);
        }

        /// <summary>
        /// The <see cref="LibraryViewModel"/> for the <see cref="GrooveHomePage"/> template.
        /// </summary>
        public LibraryViewModel? Library
        {
            get { return (LibraryViewModel)GetValue(LibraryProperty); }
            set { SetValue(LibraryProperty, value); }
        }

        /// <summary>
        /// The <see cref="GrooveHomePageViewModel"/> for the <see cref="GrooveHomePage"/> template.
        /// </summary>
        public GrooveHomePageViewModel? ViewModel
        {
            get { return (GrooveHomePageViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        private void OnLibraryChanged()
        {
            Guard.IsNotNull(Library, nameof(Library));
            ViewModel = new GrooveHomePageViewModel(Library);
        }
    }
}
