using System;
using Windows.UI;
using Windows.UI.Xaml;
using CommunityToolkit.Mvvm.ComponentModel;

namespace StrixMusic.Sdk.WinUI.Controls
{
    /// <summary>
    /// Holds the options that a shell can customize for a window host.
    /// </summary>
    public partial class ShellWindowHostOptions : ObservableObject
    {
        // TODO: Implement theme sync. This is hardcoded to work on dark mode, but needs the ability to auto-swap to light mode without overwriting custom values.
        [ObservableProperty] private Color? _backgroundColor = Colors.Transparent;
        [ObservableProperty] private Color? _buttonBackgroundColor = Colors.Transparent;
        [ObservableProperty] private Color? _buttonForegroundColor = Colors.White;
        [ObservableProperty] private Color? _buttonHoverBackgroundColor = Colors.Transparent;
        [ObservableProperty] private Color? _buttonHoverForegroundColor = Colors.White;
        [ObservableProperty] private Color? _buttonInactiveBackgroundColor = Colors.Transparent;
        [ObservableProperty] private Color? _buttonInactiveForegroundColor = Colors.White;
        [ObservableProperty] private Color? _buttonPressedBackgroundColor = Colors.Transparent;
        [ObservableProperty] private Color? _buttonPressedForegroundColor = Colors.White;
        [ObservableProperty] private Color? _foregroundColor = Colors.White;
        [ObservableProperty] private Color? _inactiveBackgroundColor = Colors.Transparent;
        [ObservableProperty] private Color? _inactiveForegroundColor = Colors.White;
        [ObservableProperty] private bool _extendViewIntoTitleBar;
        [ObservableProperty] private bool _isSystemBackButtonVisible;
        [ObservableProperty] private UIElement? _customTitleBar;

        public static Color GetColorFromHex(string hex)
        {
            hex = hex.Replace("#", string.Empty);
            var a = (byte)(Convert.ToUInt32(hex.Substring(0, 2), 16));
            var r = (byte)(Convert.ToUInt32(hex.Substring(2, 2), 16));
            var g = (byte)(Convert.ToUInt32(hex.Substring(4, 2), 16));
            var b = (byte)(Convert.ToUInt32(hex.Substring(6, 2), 16));

            return Windows.UI.Color.FromArgb(a, r, g, b);
        }
    }
}
