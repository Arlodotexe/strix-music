using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using OwlCore.Extensions;
using StrixMusic.Sdk.Data;
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

            Tracks = new ObservableCollection<GrooveTrackViewModel>();

            AttachEvents();
        }

        /// <summary>
        /// The backing dependency propery for <see cref="TrackCollection"/>.
        /// </summary>
        public static readonly DependencyProperty TrackCollectionProperty =
            DependencyProperty.Register(nameof(TrackCollection), typeof(ITrackCollectionViewModel), typeof(GrooveTrackCollection), new PropertyMetadata(null, (d, e) => _ = d.Cast<GrooveTrackCollection>().OnTrackCollectionChangedAsync()));

        /// <summary>
        /// The track collection to display.
        /// </summary>
        public ITrackCollectionViewModel? TrackCollection
        {
            get { return (ITrackCollectionViewModel)GetValue(TrackCollectionProperty); }
            set { SetValue(TrackCollectionProperty, value); }
        }

        /// <summary>
        /// The tracks displayed in the collection, with additional properties.
        /// </summary>
        public ObservableCollection<GrooveTrackViewModel> Tracks { get; set; }

        private void AttachEvents()
        {
            Unloaded += OnUnloaded;
        }

        private void DetachEvents()
        {
            if (TrackCollection != null)
                DetachEvents(TrackCollection.Tracks);

            Unloaded -= OnUnloaded;
        }

        private void AttachEvents(ObservableCollection<TrackViewModel> tracks)
        {
            tracks.CollectionChanged += Tracks_CollectionChanged;
        }

        private void DetachEvents(ObservableCollection<TrackViewModel> tracks)
        {
            tracks.CollectionChanged += Tracks_CollectionChanged;
        }

        private async Task OnTrackCollectionChangedAsync()
        {
            Tracks.Clear();

            if (TrackCollection is null)
                return;

            await TrackCollection.InitTrackCollectionAsync();

            foreach (var track in TrackCollection.Tracks)
            {
                Tracks.Add(new GrooveTrackViewModel(TrackCollection, track));
            }

            AttachEvents(TrackCollection.Tracks);
        }

        private void Tracks_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:

                    if (TrackCollection is null)
                        return;

                    foreach (var track in e.NewItems)
                        Tracks.Add(new GrooveTrackViewModel(TrackCollection, (TrackViewModel)track));

                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    for (int i = e.OldStartingIndex; i < e.OldItems.Count; i++)
                        Tracks.RemoveAt(i);

                    break;
                default:
                    break;
            }
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            DetachEvents();
        }
    }
}
