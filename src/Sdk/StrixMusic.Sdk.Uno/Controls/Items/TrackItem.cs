using OwlCore.Extensions;
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
    public partial class TrackItem : ItemControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TrackItem"/> class.
        /// </summary>
        public TrackItem()
        {
            DefaultStyleKey = typeof(TrackItem);
        }

        /// <summary>
        /// Backing dependency property for <see cref="Track"/>.
        /// </summary>
        public static readonly DependencyProperty TrackProperty =
            DependencyProperty.Register(nameof(Track), typeof(TrackViewModel), typeof(TrackItem),
                new PropertyMetadata(null, (d, e) => d.Cast<TrackItem>().OnTrackChanged((TrackViewModel?)e.OldValue, (TrackViewModel)e.NewValue)));

        /// <summary>
        /// The track to display.
        /// </summary>
        public TrackViewModel Track
        {
            get { return (TrackViewModel)GetValue(TrackProperty); }
            set { SetValue(TrackProperty, value); }
        }

        /// <inheritdoc/>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            AttachHandlers();
        }

        private void AttachHandlers()
        {
            Unloaded += TrackItem_Unloaded;
        }

        private void DetachHandlers()
        {
            Unloaded -= TrackItem_Unloaded;

            if (!(Track is null))
                Track.PlaybackStateChanged -= OnPlaybackStateChanged;
        }

        private void TrackItem_Unloaded(object sender, RoutedEventArgs e) => DetachHandlers();

        private void OnTrackChanged(TrackViewModel? oldValue, TrackViewModel newValue)
        {
            if (oldValue != null)
                oldValue.PlaybackStateChanged -= OnPlaybackStateChanged;

            if (!(newValue is null))
            {
                newValue.PlaybackStateChanged += OnPlaybackStateChanged;
                PlaybackState = newValue.PlaybackState;
            }
        }

        private void OnPlaybackStateChanged(object sender, PlaybackState e)
        {
            PlaybackState = e;
        }
    }
}
