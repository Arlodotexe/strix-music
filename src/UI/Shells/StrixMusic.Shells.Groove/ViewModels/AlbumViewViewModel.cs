using Microsoft.Toolkit.Mvvm.ComponentModel;
using StrixMusic.Sdk.ViewModels;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace StrixMusic.Shells.Groove.ViewModels
{
    public class AlbumViewViewModel : ObservableObject
    {
        private Color? _backgroundColor;

        public AlbumViewViewModel(AlbumViewModel album)
        {
            Album = album;
        }

        public AlbumViewModel Album { get; }

        public Color? BackgroundColor
        {
            get => _backgroundColor;
            set => SetProperty(ref _backgroundColor, value);
        }
    }
}
