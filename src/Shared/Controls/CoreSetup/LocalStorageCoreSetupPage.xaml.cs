using StrixMusic.Services;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Controls.CoreSetup;

/// <summary>
/// A page that displays setup for a <see cref="LocalStorageCoreSettings"/>.
/// </summary>
public sealed partial class LocalStorageCoreSetupPage : Page
{
    /// <summary>
    /// Creates a new instance of <see cref="LocalStorageCoreSetupPage"/>.
    /// </summary>
    public LocalStorageCoreSetupPage()
    {
        this.InitializeComponent();
    }
}
