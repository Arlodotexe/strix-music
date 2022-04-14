using StrixMusic.Sdk.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Sdk.WinUI.Controls.Views
{
    /// <summary>
    /// A Templated <see cref="Control"/> for showing <see cref="SearchResultsViewModel"/> as a page.
    /// </summary>
    public sealed partial class SearchView : Control
    {
        private string _query;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchView"/> class.
        /// </summary>
        public SearchView(string query)
        {
            this.DefaultStyleKey = typeof(SearchView);
            _query = query;

            // TODO: Load query results after DataContext assigned.
            //this.DataContextChanged += SearchView_DataContextChanged;
        }

        //private async void SearchView_DataContextChanged(Windows.UI.Xaml.DependencyObject sender, Windows.UI.Xaml.DataContextChangedEventArgs args)
        //{
        //    await ViewModel!.GlobalSearchResultsAsync(_query);
        //}

        /// <summary>
        /// The viewmodel that holds application's main data.
        /// </summary>
        public MainViewModel Main
        {
            get { return (MainViewModel)GetValue(MainProperty); }
            set { SetValue(MainProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="MainViewModel"/>.
        /// </summary>
        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MainProperty =
            DependencyProperty.Register("MainProperty", typeof(MainViewModel), typeof(SearchView), new PropertyMetadata(0));
    }
}
