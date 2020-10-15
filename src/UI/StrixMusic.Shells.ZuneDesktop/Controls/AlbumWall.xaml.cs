using System;
using StrixMusic.Shells.ZuneDesktop.Helpers;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace StrixMusic.Shells.ZuneDesktop.Controls
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AlbumWall : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AlbumWall"/> class.
        /// </summary>
        public AlbumWall()
        {
            this.InitializeComponent();
            Loaded += AlbumWall_Loaded;
        }

        private void AlbumWall_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= AlbumWall_Loaded;
            UpdateSize();
            int rows = (int)(ActualHeight / 72);
            int rowsAndColumns = (int)(ActualWidth / 72);
            AddRowsAndColumns(rowsAndColumns);
            TileGenerator tileGenerator = new TileGenerator(rowsAndColumns);
            foreach (var border in tileGenerator.GetTiles())
            {
                border.BorderThickness = new Thickness(1);
                border.Background = new SolidColorBrush(Colors.Cyan);
                MainGrid.Children.Add(border);
            }
        }

        private void AddRowsAndColumns(int count)
        {
            for (int i = 0; i < count; i++)
            {
                RowDefinition row = new RowDefinition();
                row.Height = new GridLength(1, GridUnitType.Star);
                ColumnDefinition column = new ColumnDefinition();
                column.Width = new GridLength(1, GridUnitType.Star);
                MainGrid.ColumnDefinitions.Add(column);
                MainGrid.RowDefinitions.Add(row);
            }
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs args)
        {
            UpdateSize();
        }

        private void UpdateSize()
        {
            if (ActualHeight > ActualWidth)
            {
                MainGrid.MinHeight = ActualHeight;
                MainGrid.MinWidth = ActualHeight;
            }
            else
            {
                MainGrid.MinHeight = ActualWidth;
                MainGrid.MinWidth = ActualWidth;
            }
        }
    }
}
