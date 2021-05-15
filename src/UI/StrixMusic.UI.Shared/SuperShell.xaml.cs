using StrixMusic.Shared.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shared
{
    /// <summary>
    /// The SuperShell is a top-level overlay that will always show on top of all other shells. It provides various essential app functions, such as changing settings, setting your shell, viewing debug info, and managing cores.
    /// </summary>
    public sealed partial class SuperShell : UserControl
    {
        /// <summary>
        /// Dependency property for <see cref="ViewModel"/>.
        /// </summary>
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(nameof(ViewModel), typeof(SuperShellViewModel), typeof(SuperShell), new PropertyMetadata(new SuperShellViewModel()));

        /// <summary>
        /// ViewModel for this control.
        /// </summary>
        public SuperShellViewModel ViewModel
        {
            get => (SuperShellViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SuperShell"/> class.
        /// </summary>
        public SuperShell()
        {
            InitializeComponent();

            _ = ViewModel.InitAsync();
        }
    }
}
