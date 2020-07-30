// <copyright file="Drawer.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Strix_Music.Shared.Strix_Style.Shell
{
    /// <summary>
    /// A Drawer used as the root of the 
    /// </summary>
    public sealed partial class Drawer : UserControl
    {
        private bool isPanelOpen = false;

        public Drawer()
        {
            this.InitializeComponent();
        }

        public object TopBarContent
        {
            get { return this.topBarContent.Content; }
            set { this.topBarContent.Content = value; }
        }

        public object PanelContent
        {
            get { return this.panelContent.Content; }
            set { this.panelContent.Content = value; }
        }

        public object BottomBarContent
        {
            get { return this.bottomBarContent.Content; }
            set { this.bottomBarContent.Content = value; }
        }

        public object BottomBarSecondaryContent
        {
            get { return this.bottomBarSecondaryContent.Content; }
            set { this.bottomBarSecondaryContent.Content = value; }
        }

        public object MainContent
        {
            get { return this.mainContent.Content; }
            set { this.mainContent.Content = value; }
        }

        private void VisualStateGroup_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            if (e.NewState == this.Mid)
            {
                this.SetPanel(false);
            }
        }

        private void TogglePanel(object sender, RoutedEventArgs args)
        {
            this.SetPanel(!this.isPanelOpen);
        }

        private void SetPanel(bool open)
        {
            VisualStateManager.GoToState(this, open ? "Closed" : "Opened", true);
            this.isPanelOpen = open;
        }
    }
}
