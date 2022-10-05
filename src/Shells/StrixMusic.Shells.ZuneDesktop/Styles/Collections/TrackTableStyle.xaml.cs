using Microsoft.Toolkit.Uwp.UI.Controls;
using StrixMusic.Sdk.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using StrixMusic.Sdk.WinUI.Controls.Collections;

namespace StrixMusic.Shells.ZuneDesktop.Styles.Collections
{
    /// <summary>
    /// A <see cref="ResourceDictionary"/> containing the style and template for the <see cref="TrackCollection"/> in the ZuneDesktop Shell.
    /// </summary>
    public sealed partial class TrackTableStyle : ResourceDictionary
    {
        private DataGrid? _grid;

        /// <summary>
        /// Initializes a new instance of the <see cref="TrackTableStyle"/> class.
        /// </summary>
        public TrackTableStyle()
        {
            this.InitializeComponent();
        }

        private void DataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            _grid = (DataGrid)sender;
        }

        private void ClearSortings(DataGrid grid)
        {
            foreach (var column in grid.Columns)
            {
                column.SortDirection = null;
            }
        }

        private void DataGrid_Sorting(object sender, DataGridColumnEventArgs e)
        {
            DataGrid grid = (DataGrid)sender;
            SortColumn(grid, e.Column);
        }

        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.DoubleTapped += Row_DoubleTapped;
        }

        private void Row_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if (_grid == null)
                return;

            var row = (DataGridRow)sender;
            ITrackCollectionViewModel viewModel = (ITrackCollectionViewModel)_grid.DataContext;
            var track = (TrackViewModel)row.DataContext;
            viewModel.PlayTrackCollectionAsync(track);
        }

        private void SortColumn(DataGrid grid, DataGridColumn column)
        {
            ITrackCollectionViewModel viewModel = (ITrackCollectionViewModel)grid.DataContext;

            TrackSortingType sortingType = TrackSortingType.Unsorted;
            SortDirection sortingDirection = SortDirection.Unsorted;

            switch (column.Tag)
            {
                case "Song":
                    sortingType = TrackSortingType.Alphanumerical;
                    break;
                case "Length":
                    sortingType = TrackSortingType.Duration;
                    break;
                case "DateAdded":
                    sortingType = TrackSortingType.DateAdded;
                    break;
                default:
                    return;
            }

            DataGridSortDirection? oldSortDirection = column.SortDirection;
            ClearSortings(grid);
            switch (oldSortDirection)
            {
                case null:
                case DataGridSortDirection.Descending:
                    column.SortDirection = DataGridSortDirection.Ascending;
                    sortingDirection = SortDirection.Ascending;
                    break;
                case DataGridSortDirection.Ascending:
                    column.SortDirection = DataGridSortDirection.Descending;
                    sortingDirection = SortDirection.Descending;
                    break;
            }

            viewModel.SortTrackCollection(sortingType, sortingDirection);
        }
    }
}
