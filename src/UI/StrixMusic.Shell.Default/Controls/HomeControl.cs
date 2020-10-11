using StrixMusic.Sdk;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shell.Default.Controls
{
    /// <summary>
    /// A Templated <see cref="Control"/> for the home page of shell.
    /// </summary>
    public sealed partial class HomeControl : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HomeControl"/> class.
        /// </summary>
        public HomeControl()
        {
            this.DefaultStyleKey = typeof(HomeControl);

            _ = MainViewModel.Singleton?.Library?.PopulateMoreTracksAsync(20);

            _ = MainViewModel.Singleton?.Library?.PopulateMoreAlbumsAsync(20);
        }

        /// <inheritdoc />
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }
    }
}
