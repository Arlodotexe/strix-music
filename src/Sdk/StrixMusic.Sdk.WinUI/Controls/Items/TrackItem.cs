using OwlCore.Extensions;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Sdk.WinUI.Controls.Items.Abstract;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Sdk.WinUI.Controls.Items
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

        /// <summary>
        /// Subscribes to the PlaybackState on track change.
        /// </summary>
        /// <param name="oldValue">The old track instance.</param>
        /// <param name="newValue">The new track instance.</param>
        public virtual void OnTrackChanged(TrackViewModel? oldValue, TrackViewModel newValue)
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
