using Microsoft.Toolkit.Mvvm.DependencyInjection;
using StrixMusic.Sdk.Uno.Controls.SubPages.Types;
using StrixMusic.Sdk.Uno.Helpers;
using StrixMusic.Sdk.Uno.Services.Localization;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Sdk.Uno.Controls.SubPages
{
    public sealed partial class CreatePlaylistPage : UserControl, ISubPage
    {
        private LocalizationResourceLoader _resourceLoader;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreatePlaylistPage"/> class.
        /// </summary>
        public CreatePlaylistPage()
        {
            this.InitializeComponent();

            _resourceLoader = Ioc.Default.GetRequiredService<LocalizationResourceLoader>();
        }

        /// <inheritdoc/>
        public string Header => _resourceLoader[Constants.Localization.MusicResource, "CreatePlaylist"];
    }
}
