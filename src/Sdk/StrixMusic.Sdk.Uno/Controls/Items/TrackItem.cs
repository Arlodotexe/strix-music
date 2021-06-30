using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Uno.Controls.Items.Abstract;
using StrixMusic.Sdk.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Sdk.Uno.Controls.Items
{
    /// <summary>
    /// A Templated <see cref="Control"/> for showing an <see cref="TrackViewModel"/> in a list.
    /// </summary>
    public sealed partial class TrackItem : ItemControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TrackItem"/> class.
        /// </summary>
        public TrackItem()
        {
            this.DefaultStyleKey = typeof(TrackItem);

            this.Loaded += TrackItem_Loaded;
        }

        private void TrackItem_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= TrackItem_Loaded;
            AttachHandlers();
        }

        private void TrackItem_Unloaded(object sender, RoutedEventArgs e)
        {
            DetachHandlers();
        }

        private void AttachHandlers()
        {
            this.Unloaded += TrackItem_Unloaded;
            ViewModel.PlaybackStateChanged += ViewModel_PlaybackStateChanged;
        }

        private void ViewModel_PlaybackStateChanged(object sender, PlaybackState e)
        {
            PlaybackState = e;
        }

        private void DetachHandlers()
        {
            this.Unloaded -= TrackItem_Unloaded;
            ViewModel.PlaybackStateChanged -= ViewModel_PlaybackStateChanged;
        }

        /// <summary>
        /// The <see cref="TrackViewModel"/> for the control.
        /// </summary>
        public TrackViewModel ViewModel => (TrackViewModel)DataContext;
    }
}
