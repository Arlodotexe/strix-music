using StrixMusic.Sdk.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.ZuneDesktop.Controls.Views.Collection
{
    /// <summary>
    /// A control that shows the properties of a Playlist.
    /// </summary>
    public sealed partial class PlaylistDetailsPane : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlaylistDetailsPane"/> class.
        /// </summary>
        public PlaylistDetailsPane()
        {
            this.InitializeComponent();
            Loaded += PlaylistDetailsPane_Loaded;
        }

        private PlaylistViewModel? ViewModel => DataContext as PlaylistViewModel;

        private void PlaylistDetailsPane_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= PlaylistDetailsPane_Loaded;
            AttachHandlers();
        }

        private void AttachHandlers()
        {
            Unloaded += PlaylistDetailsPane_Unloaded;
            DataContextChanged += PlaylistDetailsPane_DataContextChanged;
        }

        private void PlaylistDetailsPane_DataContextChanged(DependencyObject sender, DataContextChangedEventArgs args)
        {
            this.Bindings.Update();
        }

        private void PlaylistDetailsPane_Unloaded(object sender, RoutedEventArgs e)
        {
            DetachHandlers();
        }

        private void DetachHandlers()
        {
            Unloaded -= PlaylistDetailsPane_Unloaded;
            DataContextChanged -= PlaylistDetailsPane_DataContextChanged;
        }
    }
}
