using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace StrixMusic.Controls.Settings.MusicSources.ConnectNew.OneDriveCore;

/// <summary>
/// Begins the process of gathering enough data to create a OneDrive folder. Login, folder selection from root, etc.
/// </summary>
public sealed partial class LandingPage : Page
{
    /// <summary>
    /// Creates a new instance of <see cref="LandingPage"/>.
    /// </summary>
    public LandingPage()
    {
        this.InitializeComponent();
    }
}
