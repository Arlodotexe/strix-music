using System;
using CommunityToolkit.Diagnostics;
using Microsoft.Xaml.Interactivity;
using Windows.System;
using Windows.UI.Xaml;

namespace OwlCore.WinUI.Behaviors
{
    /// <summary>
    /// A behavior action that opens the provided parameter as a Uri.
    /// </summary>
    public class OpenUriAction : DependencyObject, IAction
    {
        public Uri? Source { get; set; }

        /// <inheritdoc />
        public object? Execute(object sender, object parameter)
        {
            Guard.IsNotNull(Source);

            _ = Launcher.LaunchUriAsync(Source);

            return null;
        }
    }
}
