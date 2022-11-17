using StrixMusic.AppModels;
using StrixMusic.Services;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace StrixMusic.Controls
{
    /// <summary>
    /// A control to edit an instance of <see cref="CoreSettings"/>.
    /// </summary>
    [ObservableObject]
    public sealed partial class CoreSettingsEditor : UserControl
    {
        /// <summary>
        /// Creates a new instance of <see cref="CoreSettingsEditor"/>.
        /// </summary>
        public CoreSettingsEditor()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The backing dependency property for <see cref="AppRoot"/>.
        /// </summary>
        public static readonly DependencyProperty AppRootProperty =
            DependencyProperty.Register(nameof(AppRoot), typeof(AppRoot), typeof(CoreSettingsEditor), new PropertyMetadata(null));

        /// <summary>
        /// The configured app instance for displaying configured cores.
        /// </summary>
        public AppRoot? AppRoot
        {
            get => (AppRoot?)GetValue(AppRootProperty);
            set => SetValue(AppRootProperty, value);
        }

        /// <summary>
        /// Begins the core setup process for
        /// </summary>
        [RelayCommand]
        public void StartCoreSetup(AvailableCore availableCore)
        {
            // TODO
        }
    }
}
