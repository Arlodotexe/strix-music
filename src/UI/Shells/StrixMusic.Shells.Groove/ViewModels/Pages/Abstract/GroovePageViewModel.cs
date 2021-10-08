using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace StrixMusic.Shells.Groove.ViewModels.Pages.Abstract
{
    public abstract class GroovePageViewModel<T> : ObservableObject
        where T : class
    {
        private T? _viewModel;

        protected GroovePageViewModel(T viewModel)
        {
            ViewModel = viewModel;
        }

        /// <summary>
        /// The resource key for the title.
        /// </summary>
        public abstract string PageTitleResource { get; }

        public T? ViewModel
        {
            get => _viewModel;
            set => SetProperty(ref _viewModel, value);
        }
    }
}
