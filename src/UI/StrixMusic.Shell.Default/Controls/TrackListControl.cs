using Microsoft.Toolkit.Mvvm.Input;
using OwlCore.Collections;
using OwlCore.Exceptions;
using StrixMusic.Sdk.Observables;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shell.Default.Controls
{
    /// <summary>
    /// A Templated <see cref="Control"/> for displaying any Object containing a list of <see cref="ObservableTrack"/>.
    /// </summary>
    public sealed partial class TrackListControl : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TrackListControl"/> class.
        /// </summary>
        public TrackListControl()
        {
            DefaultStyleKey = typeof(TrackListControl);
        }

        /// <summary>
        /// The backing DependencyProperty for <see cref="PopulateMoreTracksCommand"/>.
        /// </summary>
        private static readonly DependencyProperty LoadMoreTracksCommandProperty = DependencyProperty.Register(
            "PopulateMoreTracksCommand", typeof(IAsyncRelayCommand<int>), typeof(TrackListControl), new PropertyMetadata(default(IAsyncRelayCommand<int>)));

        /// <summary>
        /// The command to fire when more tracks should be loaded.
        /// </summary>
        public IAsyncRelayCommand<int> PopulateMoreTracksCommand
        {
            get => (IAsyncRelayCommand<int>) GetValue(LoadMoreTracksCommandProperty);
            set => SetValue(LoadMoreTracksCommandProperty, value);
        }

        /// <summary>
        /// The backing DependencyProperty for <see cref="Tracks"/>.
        /// </summary>
        public static readonly DependencyProperty TracksProperty = DependencyProperty.Register(
            "Tracks", typeof(SynchronizedObservableCollection<ObservableTrack>), typeof(TrackListControl), new PropertyMetadata(default(SynchronizedObservableCollection<ObservableTrack>)));

        /// <summary>
        /// The tracks for this control.
        /// </summary>
        public SynchronizedObservableCollection<ObservableTrack> Tracks
        {
            get => (SynchronizedObservableCollection<ObservableTrack>)GetValue(TracksProperty);
            set => SetValue(TracksProperty, value);
        }

        /// <summary>
        /// The main list view that holds the tracks.
        /// </summary>
        public ListView? PART_ListView { get; set; }

        /// <inheritdoc />
        protected override void OnApplyTemplate()
        {
            // OnApplyTemplate is often a more appropriate point to deal with
            // adjustments to the template-created visual tree than is the Loaded event.
            // The Loaded event might occur before the template is applied,
            // and the visual tree might be incomplete as of Loaded.
            base.OnApplyTemplate();

            PART_ListView = GetTemplateChild(nameof(PART_ListView)) as ListView ??
                            throw new UIElementNotFoundException(nameof(PART_ListView));

            // This is really, really not great
            PART_ListView.ItemsSource = Tracks;

            AttachHandlers();
        }

        private void AttachHandlers()
        {
        }

        private void DetachHandlers()
        {
        }
    }
}
