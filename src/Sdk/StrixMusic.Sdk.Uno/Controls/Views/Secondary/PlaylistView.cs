using System.Threading.Tasks;
using OwlCore.Extensions;
using StrixMusic.Sdk.ViewModels;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Sdk.Uno.Controls.Views.Secondary
{
    /// <summary>
    /// A Templated <see cref="Control"/> for showing an <see cref="PlaylistViewModel"/> as a page.
    /// </summary>
    public sealed partial class PlaylistView : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlaylistView"/> class.
        /// </summary>
        /// <param name="playlistViewModel">The Playlist in view.</param>
        public PlaylistView(PlaylistViewModel playlistViewModel)
        {
            this.DefaultStyleKey = typeof(PlaylistView);
            DataContext = playlistViewModel;

            LoadAsync().Forget();
        }

        /// <summary>
        /// The <see cref="PlaylistViewModel"/> for this control.
        /// </summary>
        public PlaylistViewModel ViewModel => (PlaylistViewModel)DataContext;

        private async Task LoadAsync()
        {
            if (!ViewModel.PopulateMoreTracksCommand.IsRunning)
                await ViewModel.PopulateMoreTracksCommand.ExecuteAsync(5);
        }
    }
}
