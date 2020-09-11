using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shell.ZuneDesktop.Helpers
{
    /// <summary>
    /// A class for generating a randomized tile layout
    /// </summary>
    public class TileGenerator
    {
        private readonly bool[,] _tiles;
        private int _currentColumn;
        private int _currentRow;
        private int _size;
        private Random _rand;

        /// <summary>
        /// Initializes a new instance of the <see cref="TileGenerator"/> class.
        /// </summary>
        /// <param name="size"></param>
        public TileGenerator(int size)
        {
            this._size = size;
            _tiles = new bool[this._size, this._size];
            _rand = new Random();
        }

        /// <summary>
        /// Gets a list of <see cref="Border"/> objects representing tiles.
        /// </summary>
        /// <returns>A list of <see cref="Border"/>s.</returns>
        public List<Border> GetTiles()
        {
            List<Border> borders = new List<Border>();
            _currentColumn = 0;
            _currentRow = 0;

            while (_currentRow != _size && _currentColumn != _size)
            {
                int maxSize = CheckSize(3);
                if (maxSize != 0)
                {
                    borders.Add(MakeTile(_currentColumn, _currentRow, maxSize, 1));
                }

                // Increments position
                _currentColumn++;
                if (_currentColumn == _size)
                {
                    _currentColumn = 0;
                    _currentRow++;
                }
            }

            return borders;
        }

        private Border MakeTile(int x, int y, int max, int min)
        {
            Border border = new Border();
            int size = _rand.Next(min, max + 1);
            border.Tag = size;
            Grid.SetColumn(border, x);
            Grid.SetRow(border, y);
            Grid.SetColumnSpan(border, size);
            Grid.SetRowSpan(border, size);

            FillTiles(
                x,
                size,
                y,
                size,
                true);
            return border;
        }

        private int CheckSize(int max)
        {
            max = Math.Min(max, _size - _currentRow);

            int i = _currentColumn;
            for (; i < _size && i < _currentColumn + max; i++)
            {
                if (_tiles[_currentRow, i])
                {
                    return i - _currentColumn;
                }
            }

            return i - _currentColumn;
        }

        private void FillTiles(int x1, int xSize, int y1, int ySize, bool value)
        {
            for (int i = y1; i < y1 + ySize; i++)
            {
                for (int j = x1; j < x1 + xSize; j++)
                {
                    _tiles[i, j] = value;
                }
            }
        }
    }
}
