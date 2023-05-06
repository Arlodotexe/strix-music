using StrixMusic.Sdk.MediaPlayback;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using StrixMusic.Sdk.WinUI.Controls.Collections.Abstract;

namespace StrixMusic.Sdk.WinUI.Controls.Items.Abstract
{
    /// <summary>
    /// Represents a container item in the <see cref="CollectionControl{TData,TItem}"/>.
    /// </summary>
    public abstract partial class ItemControl : Control
    {
        /// <summary>
        /// <see cref="DependencyProperty"/> for the <see cref="Selected"/> property.
        /// </summary>
        public static readonly DependencyProperty SelectedProperty =
            DependencyProperty.Register(
                nameof(Selected),
                typeof(bool),
                typeof(ItemControl),
                new PropertyMetadata(false));

        /// <summary>
        /// <see cref="DependencyProperty"/> for the <see cref="PlaybackState"/> property.
        /// </summary>
        public static readonly DependencyProperty PlaybackStateProperty =
            DependencyProperty.Register(
                nameof(PlaybackState),
                typeof(PlaybackState),
                typeof(ItemControl),
                new PropertyMetadata(PlaybackState.None));

        /// <summary>
        /// Gets whether or not the Item is selected.
        /// </summary>
        public bool Selected
        {
            get => (bool)GetValue(SelectedProperty);
            set
            {
                SetValue(SelectedProperty, value);
                UpdateSelectionState(value);
            }
        }

        /// <summary>
        /// Gets whether or not the Item is playing.
        /// </summary>
        public PlaybackState PlaybackState
        {
            get => (PlaybackState)GetValue(PlaybackStateProperty);
            protected set
            {
                SetValue(PlaybackStateProperty, value);
                UpdatePlayingState(value);
            }
        }

        private void UpdateSelectionState(bool selected)
        {
            if (selected)
            {
                VisualStateManager.GoToState(this, "Selected", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "Unselected", true);
            }
        }

        private void UpdatePlayingState(PlaybackState state)
        {
            switch (state)
            {
                case PlaybackState.None:
                    VisualStateManager.GoToState(this, "NotPlaying", true);
                    break;
                case PlaybackState.Failed:
                    VisualStateManager.GoToState(this, "FailedPlay", true);
                    break;
                case PlaybackState.Playing:
                    VisualStateManager.GoToState(this, "Playing", true);
                    break;
                case PlaybackState.Paused:
                    VisualStateManager.GoToState(this, "Paused", true);
                    break;
                case PlaybackState.Loaded:
                    VisualStateManager.GoToState(this, "Queued", true);
                    break;
                case PlaybackState.Loading:
                    VisualStateManager.GoToState(this, "LoadingPlay", true);
                    break;
            }
        }
    }
}
