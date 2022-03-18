using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Toolkit.Diagnostics;
using StrixMusic.Sdk.Uno.Controls.Items;
using StrixMusic.Sdk.ViewModels;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.ZuneDesktop.Controls.Views.Items
{
    /// <summary>
    /// Zune custom implemation for <see cref="AlbumItem"/>.
    /// </summary>
    public class ZuneAlbumItem : AlbumItem
    {
        /// <summary>
        /// The AlbumCollection GridView control.
        /// </summary>
        public Button? PART_PlayIcon { get; private set; }

        /// <summary>
        /// Emits the <see cref="AlbumItem"/> whose collection needs to be played.
        /// </summary>
        public event EventHandler<AlbumViewModel>? AlbumPlaybackTriggered;

        /// <summary>
        /// Creates a new instance of <see cref="ZuneAlbumItem"/>.
        /// </summary>
        public ZuneAlbumItem()
        {
        }

        /// <inheritdoc/>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            PART_PlayIcon = GetTemplateChild(nameof(PART_PlayIcon)) as Button;

            Guard.IsNotNull(PART_PlayIcon, nameof(PART_PlayIcon));
            PART_PlayIcon.Tapped += PART_PlayIcon_Tapped;
            Unloaded += ZuneAlbumItem_Unloaded;
        }

        private void ZuneAlbumItem_Unloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Guard.IsNotNull(PART_PlayIcon, nameof(PART_PlayIcon));
            PART_PlayIcon.Tapped -= PART_PlayIcon_Tapped;
        }

        private void PART_PlayIcon_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            AlbumPlaybackTriggered?.Invoke(this, Album);
        }
    }
}
