using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using StrixMusic.Sdk.ViewModels;

namespace StrixMusic.Sdk.Uno.Controls
{
    /// <summary>
    /// A Templated <see cref="Control"/> for displaying any Object containing a list of <see cref="TrackViewModel"/>.
    /// </summary>
    public sealed partial class TrackCollection : CollectionControl<TrackViewModel, TrackItem>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TrackCollection"/> class.
        /// </summary>
        public TrackCollection()
        {
            DefaultStyleKey = typeof(TrackCollection);
        }

        /// <inheritdoc />
        protected override void OnApplyTemplate()
        {
            // OnApplyTemplate is often a more appropriate point to deal with
            // adjustments to the template-created visual tree than is the Loaded event.
            // The Loaded event might occur before the template is applied,
            // and the visual tree might be incomplete as of Loaded.
            base.OnApplyTemplate();

            AttachHandlers();
        }

        private void AttachHandlers()
        {
            Unloaded += TrackCollection_Unloaded;
        }

        private void DetachHandlers()
        {
        }

        private void TrackCollection_Unloaded(object sender, RoutedEventArgs e)
        {
            DetachHandlers();
        }
    }
}
