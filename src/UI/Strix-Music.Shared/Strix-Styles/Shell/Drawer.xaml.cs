// <copyright file="Drawer.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Strix_Music.Shared.Strix_Style.Shell
{
    /// <summary>
    /// A Drawer used as the root of the Shell.
    /// </summary>
    public sealed partial class Drawer : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Drawer"/> class.
        /// </summary>
        public Drawer()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets UIElement to always show as top bar.
        /// </summary>
        public object TopBarContent
        {
            get { return this.topBarContent.Content; }
            set { this.topBarContent.Content = value; }
        }

        /// <summary>
        /// Gets or sets UIElement to always show as bottom bar.
        /// </summary>
        public object BottomBarContent
        {
            get { return this.bottomBarContent.Content; }
            set { this.bottomBarContent.Content = value; }
        }

        /// <summary>
        /// Gets or sets UIElement to always show as secondary bottom bar.
        /// </summary>
        public object BottomBarSecondaryContent
        {
            get { return this.bottomBarSecondaryContent.Content; }
            set { this.bottomBarSecondaryContent.Content = value; }
        }

        /// <summary>
        /// Gets or sets UIElement to always show as main content.
        /// </summary>
        public object MainContent
        {
            get { return this.mainContent.Content; }
            set { this.mainContent.Content = value; }
        }

        private void VisualStateGroup_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
        }
    }
}
