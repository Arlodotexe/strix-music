using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using StrixMusic.Shells.ZuneDesktop.Messages;
using NowPlayingViewWinUI = StrixMusic.Sdk.WinUI.Controls.Views.NowPlayingView;

namespace StrixMusic.Shells.ZuneDesktop.Controls.Views.NowPlayingView
{
    /// <inheritdoc cref="NowPlayingView"/>
    public class ZuneNowPlayingView : NowPlayingViewWinUI
    {
        /// <summary>
        /// Command to load nowplaying view.
        /// </summary>
        public RelayCommand BackNavigationRelayCommand { get; }

        /// <summary>
        /// Creates a new instance of <see cref="ZuneNowPlayingView"/>.
        /// </summary>
        public ZuneNowPlayingView()
        {
            this.DefaultStyleKey = typeof(ZuneNowPlayingView);

            BackNavigationRelayCommand = new RelayCommand(NavigateToPreviousScreen);
        }

        private void NavigateToPreviousScreen() => WeakReferenceMessenger.Default.Send<BackNavigationRequested>();
    }
}
