using Windows.UI.Xaml.Controls;

namespace StrixMusic.Sdk.Uno.Controls
{
    /// <summary>
    /// A Templated <see cref="Control"/> for the home page of shell.
    /// </summary>
    public sealed partial class HomeView : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HomeView"/> class.
        /// </summary>
        public HomeView()
        {
            this.DefaultStyleKey = typeof(HomeView);

            _ = MainViewModel.Singleton?.Library?.PopulateMoreTracksAsync(20);

            _ = MainViewModel.Singleton?.Library?.PopulateMoreAlbumsAsync(20);

            _ = MainViewModel.Singleton?.Library?.PopulateMoreArtistsAsync(20);
        }

        /// <inheritdoc />
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }
    }
}
