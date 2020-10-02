using Microsoft.Toolkit.Mvvm.Input;
using OwlCore.Collections;
using OwlCore.Exceptions;
using StrixMusic.Sdk.Interfaces;
using StrixMusic.Sdk.Observables;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shell.Default.Controls
{
    /// <summary>
    /// A Templated <see cref="Control"/> for displaying a list of any Observable implementing <see cref="IPlayableCollectionGroup"/>.
    /// </summary>
    /// <remarks>
    /// This class temporarily only displays <see cref="ObservableAlbum"/>s.
    /// </remarks>
    public sealed partial class PlayableCollectionListControl : Control
    {
        /// <summary>
        /// The backing DependencyProperty for <see cref="PopulateMoreCollectionsCommand"/>.
        /// </summary>
        private static readonly DependencyProperty LoadMoreCollectionsCommandProperty = DependencyProperty.Register(
            nameof(PopulateMoreCollectionsCommand),
            typeof(IAsyncRelayCommand<int>),
            typeof(AlbumViewControl),
            new PropertyMetadata(default(IAsyncRelayCommand<int>)));

        /// <summary>
        /// The backing DependencyProperty for <see cref="Collections"/>.
        /// </summary>
        public static readonly DependencyProperty CollectionsProperty = DependencyProperty.Register(
            nameof(Collections),
            typeof(SynchronizedObservableCollection<ObservableAlbum>),
            typeof(AlbumViewControl),
            new PropertyMetadata(default(SynchronizedObservableCollection<ObservableAlbum>)));

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayableCollectionListControl"/> class.
        /// </summary>
        public PlayableCollectionListControl()
        {
            this.DefaultStyleKey = typeof(PlayableCollectionListControl);
        }

        /// <summary>
        /// The command to fire when more Collections should be loaded.
        /// </summary>
        public IAsyncRelayCommand<int> PopulateMoreCollectionsCommand
        {
            get => (IAsyncRelayCommand<int>)GetValue(LoadMoreCollectionsCommandProperty);
            set => SetValue(LoadMoreCollectionsCommandProperty, value);
        }

        /// <summary>
        /// The Albums for this control.
        /// </summary>
        public SynchronizedObservableCollection<ObservableAlbum> Collections
        {
            get => (SynchronizedObservableCollection<ObservableAlbum>)GetValue(CollectionsProperty);
            set => SetValue(CollectionsProperty, value);
        }

        /// <summary>
        /// The main list view that holds the Albums.
        /// </summary>
        public GridView? PART_GridView { get; set; }

        /// <inheritdoc />
        protected override void OnApplyTemplate()
        {
            // OnApplyTemplate is often a more appropriate point to deal with
            // adjustments to the template-created visual tree than is the Loaded event.
            // The Loaded event might occur before the template is applied,
            // and the visual tree might be incomplete as of Loaded.
            base.OnApplyTemplate();

            // TODO: We don't need to do this anymore, we can bind to collection directly in XAML
            PART_GridView = GetTemplateChild(nameof(PART_GridView)) as GridView ??
                            throw new UIElementNotFoundException(nameof(PART_GridView));

            // This is really, really not great
            PART_GridView.ItemsSource = Collections;

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
