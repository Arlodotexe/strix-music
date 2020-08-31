using StrixMusic.ViewModels;
using StrixMusic.ViewModels.Bindables;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shell.Default.Controls
{
    public sealed partial class SearchViewControl : Control
    {
        private string _query;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchViewControl"/> class.
        /// </summary>
        public SearchViewControl(string query)
        {
            this.DefaultStyleKey = typeof(SearchViewControl);
            _query = query;
            this.DataContextChanged += SearchViewControl_DataContextChanged;
        }

        private async void SearchViewControl_DataContextChanged(Windows.UI.Xaml.DependencyObject sender, Windows.UI.Xaml.DataContextChangedEventArgs args)
        {
            //await ViewModel!.GlobalSearchResultsAsync(_query);
        }

        /// <summary>
        /// The <see cref="MainViewModel"/> for the app.
        /// </summary>
        public MainViewModel? ViewModel => DataContext as MainViewModel;
    }
}
