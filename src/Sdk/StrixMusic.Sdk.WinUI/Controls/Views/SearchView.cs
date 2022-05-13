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

        }

        /// <summary>
        /// The viewmodel that holds application's data root.
        /// </summary>
        public StrixDataRootViewModel Main
        {
            get { return (StrixDataRootViewModel)GetValue(MainProperty); }
            set { SetValue(MainProperty, value); }
        }

        /// <summary>
        /// Backing dependency property for <see cref="Main"/>.
        /// </summary>
        public static readonly DependencyProperty MainProperty =
            DependencyProperty.Register("MainProperty", typeof(StrixDataRootViewModel), typeof(SearchView), new PropertyMetadata(null));
    }
}
