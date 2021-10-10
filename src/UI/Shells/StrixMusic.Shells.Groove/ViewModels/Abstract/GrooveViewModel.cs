using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace StrixMusic.Shells.Groove.ViewModels.Abstract
{
    public class GrooveViewModel<T> : ObservableObject
        where T : class
    {
        private T? _viewModel;

        public GrooveViewModel(T viewModel)
        {
            _viewModel = viewModel;
        }

        public T? ViewModel
        {
            get => _viewModel;
            set => SetProperty(ref _viewModel, value);
        }
    }
}
