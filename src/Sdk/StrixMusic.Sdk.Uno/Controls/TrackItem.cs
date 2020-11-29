using Windows.UI.Xaml.Controls;
using StrixMusic.Sdk.ViewModels;

namespace StrixMusic.Sdk.Uno.Controls
{
    /// <summary>
    /// A Templated <see cref="Control"/> for showing an <see cref="TrackViewModel"/> in a list.
    /// </summary>
    public sealed partial class TrackItem : ItemControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TrackItem"/> class.
        /// </summary>
        public TrackItem()
        {
            this.DefaultStyleKey = typeof(TrackItem);
        }

        /// <summary>
        /// The <see cref="TrackViewModel"/> for the control.
        /// </summary>
        public TrackViewModel ViewModel => (DataContext as TrackViewModel)!;
    }
}
