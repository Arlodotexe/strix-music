using Microsoft.Toolkit.Mvvm.ComponentModel;
using StrixMusic.CoreInterfaces.Interfaces;

namespace StrixMusic.ViewModels.Bindables
{
    public class BindableAlbum : ObservableObject
    {
        private IAlbum _album;

        public BindableAlbum(IAlbum album)
        {
            _album = album;
        }

        public IAlbum Album
        {
            get => _album;
            set => SetProperty(ref _album, value);
        }
    }
}
