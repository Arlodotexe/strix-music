using System;
using System.Collections.Generic;
using System.Text;
using StrixMusic.Sdk.Uno.Controls.Collections;
using Windows.UI.Xaml;

namespace StrixMusic.Shells.ZuneDesktop.Controls.Views.Collection
{
    /// <summary>
    /// Zune implementation of the <see cref="ArtistCollection"/>.
    /// </summary>
    public class ZuneArtistCollection : ArtistCollection
    {
        private Queue<ZuneSortState> _sortStates;

        /// <summary>
        /// Creates a new instance of <see cref="ArtistCollection" />.
        /// </summary>
        public ZuneArtistCollection()
        {
            _sortStates = new Queue<ZuneSortState>();

            _sortStates.Enqueue(ZuneSortState.AZ);
            _sortStates.Enqueue(ZuneSortState.ZA);
        }

        /// <summary>
        /// Holds the current sort state of the zune <see cref="ArtistCollection"/>.
        /// </summary>
        public ZuneSortState SortString
        {
            get { return (ZuneSortState)GetValue(SortStringProperty); }
            set { SetValue(SortStringProperty, value); }
        }

        /// <summary>
        /// Dependency property for <ses cref="SortString" />.
        /// </summary>
        public static readonly DependencyProperty SortStringProperty =
            DependencyProperty.Register(nameof(SortString), typeof(ZuneSortState), typeof(ZuneArtistCollection), new PropertyMetadata(ZuneSortState.ZA, null));

        /// <inheritdoc />
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Collection.SortArtistCollection(Sdk.ViewModels.ArtistSortingType.Alphanumerical, Sdk.ViewModels.SortDirection.Descending);
        }
    }
}
