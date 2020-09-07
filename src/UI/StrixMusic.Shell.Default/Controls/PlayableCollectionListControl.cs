using StrixMusic.Sdk.Interfaces;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shell.Default.Controls
{
    /// <summary>
    /// A Templated <see cref="Control"/> for displaying a list of any Observable implementing <see cref="IPlayableCollectionGroup"/>.
    /// </summary>
    public sealed partial class PlayableCollectionListControl : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayableCollectionListControl"/> class.
        /// </summary>
        public PlayableCollectionListControl()
        {
            this.DefaultStyleKey = typeof(PlayableCollectionListControl);
        }
    }
}
