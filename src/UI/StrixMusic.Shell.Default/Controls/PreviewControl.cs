using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shell.Default.Controls
{
    /// <summary>
    /// A Templated <see cref="Control"/> for previewing a shell in settings.
    /// </summary>
    public sealed partial class PreviewControl : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PreviewControl"/> class.
        /// </summary>
        public PreviewControl()
        {
            this.DefaultStyleKey = typeof(PreviewControl);
        }
    }
}
