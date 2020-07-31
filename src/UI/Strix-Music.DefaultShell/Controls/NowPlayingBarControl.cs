using Windows.UI.Xaml.Controls;

namespace Strix_Music.DefaultShell.Controls
{
    public sealed partial class NowPlayingBarControl : Control
    {
        public NowPlayingBarControl()
        {
            this.DefaultStyleKey = typeof(NowPlayingBarControl);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }
    }
}
