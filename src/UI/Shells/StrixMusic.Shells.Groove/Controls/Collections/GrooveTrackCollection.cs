using OwlCore.Extensions;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.Groove.ViewModels.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.Groove.Controls.Collections
{
    public partial class GrooveTrackCollection : Control
    {
        public static readonly DependencyProperty TrackCollectionProperty =
            DependencyProperty.Register(nameof(TrackCollection), typeof(ITrackCollectionViewModel), typeof(GrooveTrackCollection), new PropertyMetadata(null, (d, e) => d.Cast<GrooveTrackCollection>().OnTrackCollectionChanged()));

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(nameof(ViewModel), typeof(GrooveTrackCollectionViewModel), typeof(GrooveTrackCollection), new PropertyMetadata(null));

        public GrooveTrackCollection()
        {
            this.DefaultStyleKey = typeof(GrooveTrackCollection);
        }

        public ITrackCollectionViewModel? TrackCollection
        {
            get { return (ITrackCollectionViewModel?)GetValue(TrackCollectionProperty); }
            set { SetValue(TrackCollectionProperty, value); }
        }

        public GrooveTrackCollectionViewModel? ViewModel
        {
            get { return (GrooveTrackCollectionViewModel?)GetValue(TrackCollectionProperty); }
            set { SetValue(TrackCollectionProperty, value); }
        }

        private void OnTrackCollectionChanged()
        {
            if (TrackCollection != null)
                ViewModel = new GrooveTrackCollectionViewModel(TrackCollection);
        } 
    }
}
