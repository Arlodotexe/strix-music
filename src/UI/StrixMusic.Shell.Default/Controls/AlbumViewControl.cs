using Microsoft.Toolkit.Mvvm.Input;
using OwlCore.Collections;
using OwlCore.Exceptions;
using StrixMusic.Sdk.Observables;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shell.Default.Controls
{
    /// <summary>
    /// A Templated <see cref="Control"/> for displaying any Object containing a list of <see cref="ObservableAlbum"/>.
    /// </summary>
    public sealed partial class AlbumViewControl : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AlbumListControl"/> class.
        /// </summary>
        public AlbumViewControl()
        {
            DefaultStyleKey = typeof(AlbumViewControl);
        }

        /// <summary>
        /// The backing DependencyProperty for <see cref="PopulateMoreAlbumsCommand"/>.
        /// </summary>
        private static readonly DependencyProperty LoadMoreAlbumsCommandProperty = DependencyProperty.Register(
            "PopulateMoreAlbumsCommand", typeof(IAsyncRelayCommand<int>), typeof(AlbumViewControl), new PropertyMetadata(default(IAsyncRelayCommand<int>)));

        /// <summary>
        /// The command to fire when more Albums should be loaded.
        /// </summary>
        public IAsyncRelayCommand<int> PopulateMoreAlbumsCommand
        {
            get => (IAsyncRelayCommand<int>)GetValue(LoadMoreAlbumsCommandProperty);
            set => SetValue(LoadMoreAlbumsCommandProperty, value);
        }

        /// <summary>
        /// The backing DependencyProperty for <see cref="Albums"/>.
        /// </summary>
        public static readonly DependencyProperty AlbumsProperty = DependencyProperty.Register(
            "Albums", typeof(SynchronizedObservableCollection<ObservableAlbum>), typeof(AlbumViewControl), new PropertyMetadata(default(SynchronizedObservableCollection<ObservableAlbum>)));

        /// <summary>
        /// The Albums for this control.
        /// </summary>
        public SynchronizedObservableCollection<ObservableAlbum> Albums
        {
            get => (SynchronizedObservableCollection<ObservableAlbum>)GetValue(AlbumsProperty);
            set => SetValue(AlbumsProperty, value);
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

            PART_GridView = GetTemplateChild(nameof(PART_GridView)) as GridView ??
                            throw new UIElementNotFoundException(nameof(PART_GridView));

            // This is really, really not great
            PART_GridView.ItemsSource = Albums;

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
