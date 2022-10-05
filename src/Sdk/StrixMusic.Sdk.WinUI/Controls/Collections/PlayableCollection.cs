using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.UI;
using OwlCore.WinUI.Collections;
using StrixMusic.Sdk.BaseModels;

namespace StrixMusic.Sdk.WinUI.Controls.Collections
{
    /// <summary>
    /// A templated <see cref="Control"/> for displaying an <see cref="StrixMusic.Sdk.BaseModels.IPlayableCollectionItem "/>.
    /// </summary>
    public abstract partial class PlayableCollection<T> : Control
    {
        /// <summary>
        /// Dependency property for <see cref="AdvancedCollectionView" />.
        /// </summary>
        public static readonly DependencyProperty AdvancedCollectionViewProperty =
            DependencyProperty.Register(nameof(AdvancedCollectionView), typeof(AdvancedCollectionView), typeof(PlayableCollection<T>), new PropertyMetadata(null));

        /// <summary>
        /// The advanced collection view used to display, sort, filter, group, and incrementally load the content.
        /// </summary>
        public AdvancedCollectionView? AdvancedCollectionView => (AdvancedCollectionView)GetValue(AdvancedCollectionViewProperty);

        /// <summary>
        /// Dependency property for <see cref="IncrementalLoader" />.
        /// </summary>
        public static readonly DependencyProperty IncrementalLoaderProperty =
            DependencyProperty.Register(nameof(IncrementalLoader), typeof(IncrementalLoadingCollection<T>), typeof(PlayableCollection<T>), new(null, (d, e) => ((PlayableCollection<T>)d).OnIncrementalLoaderChanged(e.OldValue as IncrementalLoadingCollection<T>, e.NewValue as IncrementalLoadingCollection<T>)));

        /// <summary>
        /// The collection used to incrementally load the content.
        /// </summary>
        public IncrementalLoadingCollection<T>? IncrementalLoader
        {
            get => (IncrementalLoadingCollection<T>?)GetValue(IncrementalLoaderProperty);
            protected set => SetValue(IncrementalLoaderProperty, value);
        }

        /// <summary>
        /// Dependency property for <see cref="IncrementalLoader" />.
        /// </summary>
        public static readonly DependencyProperty PageSizeProperty =
            DependencyProperty.Register(nameof(PageSize), typeof(int), typeof(PlayableCollection<T>), new(25, (d, e) => ((PlayableCollection<T>)d).OnIncrementalLoaderChanged(e.OldValue as IncrementalLoadingCollection<T>, e.NewValue as IncrementalLoadingCollection<T>)));

        /// <summary>
        /// The size of each page that is loaded.
        /// </summary>
        public int PageSize
        {
            get => (int)GetValue(PageSizeProperty);
            protected set => SetValue(PageSizeProperty, value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayableCollection{T}"/> class.
        /// </summary>
        public PlayableCollection()
        {
            // Allows directly using this control as the x:DataType in the template.
            DataContext = this;
        }

        protected virtual void OnIncrementalLoaderChanged(IncrementalLoadingCollection<T>? oldValue, IncrementalLoadingCollection<T>? newValue)
        {
            if (newValue is null)
                SetValue(AdvancedCollectionViewProperty, null);

            if (newValue is not null)
                SetValue(AdvancedCollectionViewProperty, new AdvancedCollectionView(, true));
        }
    }
}
