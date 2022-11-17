using OwlCore.Storage.Uwp;
using StrixMusic.AppModels;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace StrixMusic;

/// <summary>
/// A control to display top-level app content.
/// </summary>
public sealed partial class AppFrame : UserControl
{
    /// <summary>
    /// Creates a new instance of <see cref="AppFrame"/>.
    /// </summary>
    public AppFrame()
    {
        InitializeComponent();

        AppRoot = new AppRoot(new WindowsStorageFolder(ApplicationData.Current.LocalFolder));
    }

    /// <summary>
    /// The root for all data required by the Strix Music App to function.
    /// </summary>
    public AppRoot AppRoot { get; }
}
