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
        }

        /// <summary>
        /// The <see cref="TrackViewModel"/> for the control.
        /// </summary>
        public TrackViewModel ViewModel => (TrackViewModel)DataContext;

        /// <inheritdoc/>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            AttachHandlers();
        }

        private void AttachHandlers()
        {
            this.Unloaded += TrackItem_Unloaded;
            this.DataContextChanged += TrackItem_DataContextChanged;
        }

        private void DetachHandlers()
        {
            this.Unloaded -= TrackItem_Unloaded;
            this.DataContextChanged -= TrackItem_DataContextChanged;

            if (ViewModel != null)
            {
                ViewModel.PlaybackStateChanged -= ViewModel_PlaybackStateChanged;
            }
        }

        private void TrackItem_DataContextChanged(DependencyObject sender, DataContextChangedEventArgs args)
        {
            if (DataContext != null)
            {
                ViewModel.PlaybackStateChanged -= ViewModel_PlaybackStateChanged;
            }

            if (args.NewValue is TrackViewModel viewModel)
            {
                viewModel.PlaybackStateChanged += ViewModel_PlaybackStateChanged;
                PlaybackState = viewModel.PlaybackState;
            }
        }

        private void ViewModel_PlaybackStateChanged(object sender, PlaybackState e)
        {
            PlaybackState = e;
        }

        private void TrackItem_Unloaded(object sender, RoutedEventArgs e)
        {
            DetachHandlers();
        }
    }
}
