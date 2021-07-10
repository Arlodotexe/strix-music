using StrixMusic.Sdk.Uno.Controls.SubPages.Types;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Sdk.Uno.Controls.SubPages
{
    public sealed partial class CreatePlaylistPage : UserControl, ISubPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreatePlaylistPage"/> class.
        /// </summary>
        public CreatePlaylistPage()
        {
            this.InitializeComponent();
        }

        /// <inheritdoc/>
        public string Header => "Create Playlist";
    }
}
