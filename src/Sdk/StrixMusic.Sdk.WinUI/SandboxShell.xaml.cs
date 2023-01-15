using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using StrixMusic.Sdk.WinUI.Controls;

namespace StrixMusic.Sdk.WinUI
{
    /// <summary>
    /// An extremely basic shell that serves to exercise the components included in the Strix WinUI SDK.
    /// </summary>
    public sealed partial class SandboxShell : Shell
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SandboxShell"/> class.
        /// </summary>
        public SandboxShell()
        {
            this.InitializeComponent();
        }

        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            // TODO
        }
    }
}
