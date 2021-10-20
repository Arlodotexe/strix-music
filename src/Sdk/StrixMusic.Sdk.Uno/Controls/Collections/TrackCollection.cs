using StrixMusic.Sdk.Uno.Controls.Collections.Abstract;
using StrixMusic.Sdk.Uno.Controls.Items;
using StrixMusic.Sdk.ViewModels;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Sdk.Uno.Controls.Collections
{
    /// <summary>
    /// A Templated <see cref="Control"/> for displaying an <see cref="ITrackCollectionViewModel"/>.
    /// </summary>
    public partial class TrackCollection : CollectionControl<TrackViewModel, TrackItem>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TrackCollection"/> class.
        /// </summary>
        public TrackCollection()
        {
            DefaultStyleKey = typeof(TrackCollection);
        }

        /// <summary>
        /// The <see cref="ITrackCollectionViewModel"/> for the control.
        /// </summary>
        public ITrackCollectionViewModel? ViewModel => DataContext as ITrackCollectionViewModel;

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

        /// <inheritdoc/>
        protected override async Task LoadMore()
        {
            if (ViewModel == null)
                return;

            if (!ViewModel.PopulateMoreTracksCommand.IsRunning)
                await ViewModel.PopulateMoreTracksCommand.ExecuteAsync(25);
        }

        /// <inheritdoc/>
        protected override void CheckAndToggleEmpty()
        {
            if (ViewModel == null)
                return;

            if (!ViewModel.PopulateMoreTracksCommand.IsRunning &&
                ViewModel.TotalTrackCount == 0)
                
            SetEmptyVisibility(Visibility.Visible);
        }

        private void AttachHandlers()
        {
            Unloaded += TrackCollection_Unloaded;
        }

        private void DetachHandlers()
        {
            Unloaded -= TrackCollection_Unloaded;
        }

        private void TrackCollection_Unloaded(object sender, RoutedEventArgs e)
        {
            DetachHandlers();
        }
    }
}
