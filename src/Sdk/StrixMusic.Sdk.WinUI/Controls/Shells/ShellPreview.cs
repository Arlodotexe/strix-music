using Windows.UI.Xaml.Controls;

namespace StrixMusic.Sdk.WinUI.Controls.Shells
{
    /// <summary>
    /// A Templated <see cref="Control"/> for previewing a shell in settings.
    /// </summary>
    public sealed partial class ShellPreview : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShellPreview"/> class.
        /// </summary>
        public ShellPreview()
        {
            this.DefaultStyleKey = typeof(ShellPreview);
        }
    }
}
