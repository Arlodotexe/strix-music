using Microsoft.Toolkit.Uwp.UI.Controls;
using StrixMusic.Sdk.ViewModels;
using System;
using Windows.UI.Xaml;

namespace StrixMusic.Shells.ZuneDesktop.Styles.Collections
{
    /// <summary>
    /// A <see cref="ResourceDictionary"/> containing the style and template for the <see cref="Sdk.Uno.Controls.TrackCollection"/> in the ZuneDesktop Shell.
    /// </summary>
    public sealed partial class TrackTableStyle : ResourceDictionary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TrackTableStyle"/> class.
        /// </summary>
        public TrackTableStyle()
        {
            this.InitializeComponent();
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
            DataGridSortDirection? oldSortDirection = e.Column.SortDirection;
            ClearSortings((DataGrid)sender);

            switch (oldSortDirection)
            {
                case null:
                    e.Column.SortDirection = DataGridSortDirection.Ascending;
                    break;
                case DataGridSortDirection.Ascending:
                    e.Column.SortDirection = DataGridSortDirection.Descending;
                    break;
                case DataGridSortDirection.Descending:
                    e.Column.SortDirection = null;
                    break;
            }
        }
    }
}
