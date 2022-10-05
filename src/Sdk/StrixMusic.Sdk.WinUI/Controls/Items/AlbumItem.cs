using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Sdk.WinUI.Controls.Items.Abstract;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Sdk.WinUI.Controls.Items
{
    /// <summary>
    /// A Templated <see cref="Control"/> for displaying an <see cref="IAlbum"/> in a list.
    /// </summary>
    public partial class AlbumItem : ItemControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AlbumItem"/> class.
        /// </summary>
        public AlbumItem()
        {
            this.DefaultStyleKey = typeof(AlbumItem);
            AttachEvents();
        }

        private void AttachEvents()
        {
            Loaded += AlbumItem_Loaded;
            Unloaded += AlbumItem_Unloaded;
        }

        private void DetachEvents()
        {
            Unloaded -= AlbumItem_Unloaded;
        }

        /// <summary>
        /// Dependency property for <see cref="Album"/>.
        /// </summary>
        public static readonly DependencyProperty AlbumProperty =
            DependencyProperty.Register(nameof(Album), typeof(IAlbum), typeof(AlbumItem), new PropertyMetadata(null, (d, e) => ((AlbumItem)d).OnAlbumChanged(e.OldValue as IAlbum, e.NewValue as IAlbum)));

        /// <summary>
        /// Dependency property for <see cref="AlbumVm"/>.
        /// </summary>
        public static readonly DependencyProperty AlbumViewModelProperty =
            DependencyProperty.Register(nameof(AlbumVm), typeof(AlbumViewModel), typeof(AlbumItem), new PropertyMetadata(null));

        /// <summary>
        /// ViewModel holding the data for <see cref="AlbumItem" />
        /// </summary>
        public IAlbum? Album
        {
            get => (IAlbum)GetValue(AlbumProperty);
            set => SetValue(AlbumProperty, value);
        }

        /// <summary>
        /// A view model version of <see cref="Album"/>.
        /// </summary>
        public AlbumViewModel? AlbumVm => (AlbumViewModel)GetValue(AlbumViewModelProperty);

        private void AlbumItem_Unloaded(object sender, RoutedEventArgs e)
        {
            DetachEvents();
        }

        private void AlbumItem_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= AlbumItem_Loaded;
        }

        /// <summary>
        /// Fires when the <see cref="Album"/> is changed.
        /// </summary>
        protected virtual void OnAlbumChanged(IAlbum? oldValue, IAlbum? newValue)
        {
            if (newValue is not null)
                SetValue(AlbumViewModelProperty, Album as AlbumViewModel ?? new AlbumViewModel(newValue, newValue.Root));
        }
    }
}
