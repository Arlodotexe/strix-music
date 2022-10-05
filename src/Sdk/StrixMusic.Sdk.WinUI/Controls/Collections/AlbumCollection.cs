using System.Linq;
using StrixMusic.Sdk.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Toolkit.Uwp;
using OwlCore;
using OwlCore.WinUI.Collections;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.BaseModels;
using StrixMusic.Sdk.WinUI.Controls.Views.Secondary;

namespace StrixMusic.Sdk.WinUI.Controls.Collections
{

    /// <summary>
    /// A templated <see cref="Control"/> for displaying an <see cref="IAlbumCollectionViewModel"/>.
    /// </summary>
    public partial class AlbumCollection : PlayableCollection<AlbumViewModel>
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
        /// Dependency property for <see cref="IAlbumCollectionViewModel" />.
        /// </summary>
        public static readonly DependencyProperty CollectionProperty =
            DependencyProperty.Register(nameof(Collection), typeof(IAlbumCollectionViewModel), typeof(AlbumCollection), new PropertyMetadata(null, (s, e) => ((AlbumCollection)s).OnCollectionChanged((IAlbumCollectionViewModel?)e.OldValue, (IAlbumCollectionViewModel?)e.NewValue)));

        /// <summary>
        /// The album collection to display.
        /// </summary>
        public IAlbumCollectionViewModel? Collection
        {
            get => (IAlbumCollectionViewModel)GetValue(AdvancedCollectionViewProperty);
            set => SetValue(AdvancedCollectionViewProperty, value);
        }

        /// <summary>
        /// Fires when the <see cref="Collection"/> property changes.
        /// </summary>
        protected virtual void OnCollectionChanged(IAlbumCollectionViewModel? oldValue, IAlbumCollectionViewModel? newValue)
        {
            if (newValue is not null)
            {
                IncrementalLoader = new IncrementalLoadingCollection<AlbumViewModel>(async x => await newValue.GetAlbumItemsAsync(25, x).ToListAsync().Where(x => x is IAlbum));
            }
        }
    }
}
