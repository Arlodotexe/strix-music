using OwlCore.Extensions;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.Groove.ViewModels.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.Groove.Controls.Collections
{
    /// <summary>
    /// A <see cref="Control"/> for displaying <see cref="TrackCollectionViewModel"/>s in the Groove Shell.
    /// </summary>
    public partial class GrooveTrackCollection : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GrooveTrackCollection"/> class.
        /// </summary>
        public GrooveTrackCollection()
        {
            DefaultStyleKey = typeof(GrooveTrackCollection);
            DataContext = new GrooveTrackCollectionViewModel();
        }

        /// <summary>
        /// The ViewModel for a <see cref="GrooveTrackCollection"/>
        /// </summary>
        public GrooveTrackCollectionViewModel ViewModel => (GrooveTrackCollectionViewModel)DataContext;

        /// <summary>
        /// The backing dependency propery for <see cref="TrackCollection"/>.
        /// </summary>
        public static readonly DependencyProperty TrackCollectionProperty =
            DependencyProperty.Register(nameof(TrackCollection), typeof(ITrackCollectionViewModel), typeof(GrooveTrackCollection), new PropertyMetadata(null, (d, e) => d.Cast<GrooveTrackCollection>().OnTrackCollectionChanged()));

        /// <summary>
        /// The track collection to display.
        /// </summary>
        public ITrackCollectionViewModel TrackCollection
        {
            get { return (ITrackCollectionViewModel)GetValue(TrackCollectionProperty); }
            set { SetValue(TrackCollectionProperty, value); }
        }

        private void OnTrackCollectionChanged()
        {
            ViewModel.TrackCollection = TrackCollection;
        }
    }
}
