using System;
using System.Threading;
using Windows.System.Threading;
using Windows.UI.Xaml;

namespace StrixMusic.Sdk.WinUI.Controls.NowPlaying
{
    /// <summary>
    /// A slider meant to be used for media playback.
    /// </summary>
    public partial class MediaSlider : SliderEx
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MediaSlider"/> class.
        /// </summary>
        public MediaSlider()
        {
            DefaultStyleKey = typeof(MediaSlider);
        }
    }
}
