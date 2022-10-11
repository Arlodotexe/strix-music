﻿using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using StrixMusic.Sdk.WinUI.Controls;

namespace StrixMusic.Shells.Default
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DefaultShell : Shell
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultShell"/> class.
        /// </summary>
        public DefaultShell()
        {
        }

        private void DefaultShell_Unloaded(object sender, RoutedEventArgs e)
        {
            Unloaded -= DefaultShell_Unloaded;
        }

        private void Shell_BackRequested(object sender, EventArgs e)
        {
            if (OverlayContent.Visibility == Visibility.Visible)
            {
                OverlayContent.Visibility = Visibility.Collapsed;
                return;
            }
            
            // TODO
        }

        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            // TODO
        }

        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            // TODO
        }
    }
}
