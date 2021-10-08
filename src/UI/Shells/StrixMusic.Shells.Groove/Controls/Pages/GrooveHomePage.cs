using StrixMusic.Shells.Groove.Controls.Pages.Abstract;
using StrixMusic.Shells.Groove.ViewModels.Pages;
using Windows.UI.Xaml;

namespace StrixMusic.Shells.Groove.Controls.Pages
{
    public partial class GrooveHomePage : GroovePageControl<HomeViewViewModel>
    {
        /// <summary>
        /// The backing dependency property for <see cref="ViewModel"/>.
        /// </summary>
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(nameof(ViewModel), typeof(HomeViewViewModel), typeof(GrooveHomePage), new PropertyMetadata(null));

        public GrooveHomePage()
        {
            this.DefaultStyleKey = typeof(GrooveHomePage);
        }

        /// <summary>
        /// The <see cref="HomeViewViewModel"/> for the <see cref="GrooveHomePage"/> template.
        /// </summary>
        public HomeViewViewModel? ViewModel
        {
            get { return (HomeViewViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
    }
}
