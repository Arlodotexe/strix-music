using OwlCore.Extensions;
using StrixMusic.Sdk.ViewModels;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Sdk.Uno.Controls
{
    /// <summary>
    /// A Templated <see cref="Control"/> for displaying an <see cref="PlaylistViewModel"/> in a list.
    /// </summary>
    public sealed partial class PlaylistItem : ItemControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlaylistItem"/> class.
        /// </summary>
        public PlaylistItem()
        {
            this.DefaultStyleKey = typeof(PlaylistItem);

            InitAsync().FireAndForget();
        }

        private async Task InitAsync()
        {
            if (!ViewModel.PopulateMoreTracksCommand.IsRunning)
                await ViewModel.PopulateMoreTracksCommand.ExecuteAsync(5);
        }

        /// <summary>
        /// The <see cref="PlaylistViewModel"/> for the control.
        /// </summary>
        public PlaylistViewModel ViewModel => (PlaylistViewModel)DataContext;
    }
}
