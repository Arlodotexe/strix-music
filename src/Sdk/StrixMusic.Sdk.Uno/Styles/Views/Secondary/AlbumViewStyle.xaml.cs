using Microsoft.Toolkit.Diagnostics;
using StrixMusic.Sdk.Services.Navigation;
using StrixMusic.Sdk.Uno.Controls.Shells;
using StrixMusic.Sdk.Uno.Controls.Views.Secondary;
using StrixMusic.Sdk.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Sdk.Uno.Styles.Views.Secondary
{
    /// <summary>
    /// A <see cref="ResourceDictionary"/> containing the default style for the <see cref="AlbumView"/>.
    /// </summary>
    public sealed partial class AlbumViewStyle : ResourceDictionary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AlbumStyle"/> class.
        /// </summary>
        public AlbumViewStyle()
        {
            this.InitializeComponent();
        }


        /// <summary>
        /// ViewModel holding the data for <see cref="AlbumItem" />
        /// </summary>
        public AlbumViewModel Album
        {
            get { return (AlbumViewModel)GetValue(AlbumProperty); }
            set { SetValue(AlbumProperty, value); }
        }

        /// <summary>
        /// Dependency property for <ses cref="AlbumViewModel" />.
        /// </summary>
        // Using a DependencyProperty as the backing store for ViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AlbumProperty =
            DependencyProperty.Register(nameof(Album), typeof(AlbumViewModel), typeof(AlbumViewStyle), new PropertyMetadata(0));

        private void GoToArtist(object sender, RoutedEventArgs e)
        {
            if ((sender as Control)?.DataContext is ArtistViewModel viewModel)
            {
                INavigationService<Control> navigationService = Shell.Ioc.GetService<INavigationService<Control>>() ?? ThrowHelper.ThrowInvalidOperationException<INavigationService<Control>>();

                navigationService.NavigateTo(typeof(ArtistView), false, viewModel);
            }
        }
    }
}
