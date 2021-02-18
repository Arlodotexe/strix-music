using StrixMusic.Sdk.Uno.Controls.Items.Abstract;
using StrixMusic.Sdk.ViewModels;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Sdk.Uno.Controls.Items
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
