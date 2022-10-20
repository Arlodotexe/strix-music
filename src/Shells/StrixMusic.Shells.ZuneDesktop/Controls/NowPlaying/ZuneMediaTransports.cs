using System;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using StrixMusic.Sdk.WinUI.Controls.NowPlaying;
using StrixMusic.Shells.ZuneDesktop.Messages.Pages;

namespace StrixMusic.Shells.ZuneDesktop.Controls.NowPlaying
{
    /// <summary>
    /// The Media Transparent controls for the ZuneDesktop.
    /// </summary>
    public partial class ZuneMediaTransports : MediaTransports
    {
        /// <summary>
        /// Command to load nowplaying view.
        /// </summary>
        public RelayCommand NowPlayingViewCommand { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZuneMediaTransports"/> class.
        /// </summary>
        public ZuneMediaTransports()
        {
            this.DefaultStyleKey = typeof(ZuneMediaTransports);

            NowPlayingViewCommand = new RelayCommand(NavigateToNowPlayingView);
        }

        private void NavigateToNowPlayingView() => WeakReferenceMessenger.Default.Send<NowPlayingViewNavigationRequestMessage>();
    }
}
