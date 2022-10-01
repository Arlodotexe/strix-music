using System;
using StrixMusic.Sdk.AppModels;
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
        /// Dependency property for <see cref="Track"/>.
        /// </summary>
        public static readonly DependencyProperty TrackProperty =
            DependencyProperty.Register(nameof(Track), typeof(ITrack), typeof(TrackItem), new PropertyMetadata(null, (d, e) => ((TrackItem)d).OnTrackChanged(e.OldValue as ITrack, e.NewValue as ITrack)));

        /// <summary>
        /// Dependency property for <see cref="TrackVm"/>.
        /// </summary>
        public static readonly DependencyProperty TrackViewModelProperty =
            DependencyProperty.Register(nameof(Track), typeof(TrackViewModel), typeof(TrackItem), new PropertyMetadata(null));

        /// <summary>
        /// ViewModel holding the data for <see cref="TrackItem" />
        /// </summary>
        public ITrack? Track
        {
            get => (ITrack)GetValue(TrackProperty);
            set => SetValue(TrackProperty, value);
        }

        /// <summary>
        /// The track to display.
        /// </summary>
        public TrackViewModel TrackVm => (TrackViewModel)GetValue(TrackViewModelProperty);

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
        /// Fires when the <see cref="Track"/> is changed.
        /// </summary>
        protected virtual void OnTrackChanged(ITrack? oldValue, ITrack? newValue)
        {
            if (newValue is not null)
                SetValue(TrackViewModelProperty, Track is TrackViewModel albumVm ? albumVm : new TrackViewModel(newValue, newValue.Root));

            if (oldValue is not null)
                oldValue.PlaybackStateChanged -= OnPlaybackStateChanged;

            if (newValue is not null)
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
