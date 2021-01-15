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

            if (MainViewModel.Singleton?.Library?.PopulateMoreTracksCommand.IsRunning == false)
                _ = MainViewModel.Singleton?.Library?.PopulateMoreTracksCommand.ExecuteAsync(60);

            if (MainViewModel.Singleton?.Library?.PopulateMoreAlbumsCommand.IsRunning == false)
                _ = MainViewModel.Singleton?.Library?.PopulateMoreAlbumsCommand.ExecuteAsync(60);

            if (MainViewModel.Singleton?.Library?.PopulateMoreArtistsCommand.IsRunning == false)
                _ = MainViewModel.Singleton?.Library?.PopulateMoreArtistsCommand.ExecuteAsync(60);

            if (MainViewModel.Singleton?.Library?.PopulateMorePlaylistsCommand.IsRunning == false)
                _ = MainViewModel.Singleton?.Library?.PopulateMorePlaylistsCommand.ExecuteAsync(60);
        }

        /// <inheritdoc />
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }
    }
}
