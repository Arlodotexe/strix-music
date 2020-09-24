using Microsoft.Toolkit.Mvvm.Input;
using StrixMusic.Sdk.Observables;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using OwlCore.Exceptions;

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
        /// The backing DependencyProperty for <see cref="LoadMoreTracksCommand"/>.
        /// </summary>
        private static readonly DependencyProperty LoadMoreTracksCommandProperty = DependencyProperty.Register(
            "LoadMoreTracksCommand", typeof(AsyncRelayCommand), typeof(TrackListControl), new PropertyMetadata(default(AsyncRelayCommand)));

        /// <summary>
        /// The command to fire when more tracks should be loaded.
        /// </summary>
        public AsyncRelayCommand LoadMoreTracksCommand
        {
            get => (AsyncRelayCommand) GetValue(LoadMoreTracksCommandProperty);
            set => SetValue(LoadMoreTracksCommandProperty, value);
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
