using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using StrixMusic.Sdk.AppModels;

namespace StrixMusic.Sdk.WinUI.Controls.Views
{
    /// <summary>
    /// A Templated <see cref="Control"/> for the home page of shell.
    /// </summary>
    public sealed partial class HomeView : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HomeView"/> class.
        /// </summary>
        public HomeView()
        {
            this.DefaultStyleKey = typeof(HomeView);
            DataContext = this;
        }

        /// <summary>
        /// The root object containing all data needed to power strix.
        /// </summary>
        public IStrixDataRoot? DataRoot
        {
            get => (IStrixDataRoot?)GetValue(DataRootProperty);
            set => SetValue(DataRootProperty, value);
        }

        /// <summary>
        /// Backing dependency property for <see cref="DataRoot"/>.
        /// </summary>
        public static readonly DependencyProperty DataRootProperty =
            DependencyProperty.Register(nameof(DataRoot), typeof(IStrixDataRoot), typeof(HomeView), new PropertyMetadata(null));

        /// <inheritdoc />
        protected override void OnApplyTemplate() => base.OnApplyTemplate();
    }
}
