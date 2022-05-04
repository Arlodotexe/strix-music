using System;
using System.Collections.Generic;
using System.Text;
using StrixMusic.Sdk.WinUI.Controls.Collections;

namespace StrixMusic.Shells.ZuneDesktop.Controls.Views.Collection
{
    /// <summary>
    /// Zune implmenation for the <see cref="ZuneTrackCollection"/>.
    /// </summary>
    public class ZuneTrackCollection : TrackCollection
    {
        /// <summary>
        /// Creates a new instace for <see cref="ZuneTrackCollection"/>.
        /// </summary>
        public ZuneTrackCollection()
        {
        }

        /// <inheritdoc/>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }
    }
}
