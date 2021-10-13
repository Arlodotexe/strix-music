using Microsoft.Toolkit.Mvvm.Messaging;
using StrixMusic.Shells.Groove.Messages.Navigation;
using StrixMusic.Shells.Groove.Messages.Navigation.Pages;
using StrixMusic.Shells.Groove.Messages.Navigation.Pages.Abstract;
using System.Collections.Generic;

namespace StrixMusic.Shells.Groove.Helper
{
    /// <summary>
    /// A class for tracking navigation history.
    /// </summary>
    public class NavigationTracker
    {
        private Stack<PageNavigationRequestedMessage>? _navigationStack;

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationTracker"/> class.
        /// </summary>
        public void Initialize()
        {
            _navigationStack = new Stack<PageNavigationRequestedMessage>();

            WeakReferenceMessenger.Default.Register<BackNavigationRequested>(this,
                (s, e) => NavigateBackwards());

            WeakReferenceMessenger.Default.Register<AlbumViewNavigationRequested>(this,
                (s, e) => RecordNavigation(e));
            WeakReferenceMessenger.Default.Register<ArtistViewNavigationRequested>(this,
                (s, e) => RecordNavigation(e));
            WeakReferenceMessenger.Default.Register<HomeViewNavigationRequested>(this,
                (s, e) => RecordNavigation(e));
            WeakReferenceMessenger.Default.Register<PlaylistViewNavigationRequested>(this,
                (s, e) => RecordNavigation(e));
            WeakReferenceMessenger.Default.Register<PlaylistsViewNavigationRequested>(this,
                (s, e) => RecordNavigation(e));
        }

        /// <summary>
        /// Gets the singleton instance of this class.
        /// </summary>
        public static NavigationTracker Instance { get; } = new NavigationTracker();

        /// <summary>
        /// Gets whether or not the navigation can go backwards.
        /// </summary>
        public bool CanGoBack => _navigationStack != null ? _navigationStack.Count > 1 : false;

        public void NavigateBackwards()
        {
            if (!CanGoBack || _navigationStack == null)
                return;

            _navigationStack.Pop();
            PageNavigationRequestedMessage previous = _navigationStack.Peek();
            previous.RecordNavigation = false;

            switch (previous)
            {
                case AlbumViewNavigationRequested albumViewNavigationRequest:
                    WeakReferenceMessenger.Default.Send(albumViewNavigationRequest);
                    break;
                case ArtistViewNavigationRequested artistViewNavigationRequest:
                    WeakReferenceMessenger.Default.Send(artistViewNavigationRequest);
                    break;
                case HomeViewNavigationRequested homeViewNavigationRequest:
                    WeakReferenceMessenger.Default.Send(homeViewNavigationRequest);
                    break;
                case PlaylistsViewNavigationRequested playlistViewNavigationRequest:
                    WeakReferenceMessenger.Default.Send(playlistViewNavigationRequest);
                    break;
            }
        }

        private void RecordNavigation<T>(T viewModel)
            where T : PageNavigationRequestedMessage
        {
            if (_navigationStack == null)
                return;

            if (viewModel.RecordNavigation)
                _navigationStack.Push(viewModel);
        }
    }
}
