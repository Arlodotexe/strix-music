using System.Threading.Tasks;
using OwlCore.Extensions;
using StrixMusic.Sdk.WinUI.Controls.Items;
using StrixMusic.Sdk.WinUI.Controls.Collections.Abstract;
using StrixMusic.Sdk.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Sdk.WinUI.Controls.Collections
{
    /// <summary>
    /// A templated <see cref="Control"/> for displaying an <see cref="IAlbumCollectionViewModel"/>.
    /// </summary>
    /// <remarks>
    /// This class temporarily only displays <see cref="AlbumViewModel"/>s.
    /// </remarks>
    public partial class AlbumCollection : CollectionControl<AlbumViewModel, AlbumItem>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AlbumCollection"/> class.
        /// </summary>
        public AlbumCollection()
        {
            this.DefaultStyleKey = typeof(AlbumCollection);
        }

        /// <inheritdoc />
        protected override void OnApplyTemplate()
        {
            // OnApplyTemplate is often a more appropriate point to deal with
            // adjustments to the template-created visual tree than is the Loaded event.
            // The Loaded event might occur before the template is applied,
            // and the visual tree might be incomplete as of Loaded.
            base.OnApplyTemplate();

            OnCollectionChanged(null, Collection);

            AttachHandlers();
        }

        /// <inheritdoc/>
        protected override async Task LoadMore()
        {
            if (Collection is not null && !Collection.PopulateMoreAlbumsCommand.IsRunning)
                await Collection.PopulateMoreAlbumsCommand.ExecuteAsync(25);
        }

        /// <inheritdoc/>
        protected override void CheckAndToggleEmpty()
        {
            if (Collection is not null &&
                !Collection.PopulateMoreAlbumsCommand.IsRunning &&
                Collection.TotalAlbumItemsCount == 0)
                SetEmptyVisibility(Visibility.Visible);
        }

        private void AttachHandlers()
        {
            Unloaded += AlbumCollection_Unloaded;
        }

        private void AlbumCollection_Unloaded(object sender, RoutedEventArgs e)
        {
            DetachHandlers();
        }

        private void DetachHandlers()
        {
            Unloaded -= AlbumCollection_Unloaded;
        }

        /// <summary>
        /// Collection holding the data for <see cref="AlbumCollection" />
        /// </summary>
        public IAlbumCollectionViewModel? Collection
        {
            get { return (IAlbumCollectionViewModel)GetValue(CollectionProperty); }
            set { SetValue(CollectionProperty, value); }
        }

        /// <summary>
        /// Dependency property for <ses cref="IAlbumCollectionViewModel" />.
        /// </summary>
        public static readonly DependencyProperty CollectionProperty =
            DependencyProperty.Register(nameof(Collection), typeof(IAlbumCollectionViewModel), typeof(AlbumCollection), new PropertyMetadata(null, (s, e) => s.Cast<AlbumCollection>().OnCollectionChanged((IAlbumCollectionViewModel?)e.OldValue, (IAlbumCollectionViewModel?)e.NewValue)));

        /// <summary>
        /// Fires when the <see cref="Collection"/> property changes.
        /// </summary>
        protected virtual void OnCollectionChanged(IAlbumCollectionViewModel? oldValue, IAlbumCollectionViewModel? newValue)
        {
            if (newValue is not null && !newValue.PopulateMoreAlbumsCommand.IsRunning && newValue.Albums.Count == 0)
                newValue.PopulateMoreAlbumsCommand.Execute(20);
        }
    }
}
