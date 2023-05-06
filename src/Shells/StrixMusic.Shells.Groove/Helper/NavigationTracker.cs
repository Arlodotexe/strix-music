using System.Collections.Generic;
using CommunityToolkit.Mvvm.Messaging;
using StrixMusic.Shells.Groove.Messages.Navigation;
using StrixMusic.Shells.Groove.Messages.Navigation.Pages;
using StrixMusic.Shells.Groove.Messages.Navigation.Pages.Abstract;

namespace StrixMusic.Shells.Groove.Helper
{
    /// <summary>
    /// A class for tracking navigation history.
    /// </summary>
    public class NavigationTracker
    {
        private Stack<PageNavigationRequestMessage>? _navigationStack;

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationTracker"/> class.
        /// </summary>
        public void Initialize()
        {
            _navigationStack = new Stack<PageNavigationRequestMessage>();

            WeakReferenceMessenger.Default.Register<BackNavigationRequested>(this,
                (s, e) => NavigateBackwards());

            WeakReferenceMessenger.Default.Register<AlbumViewNavigationRequestMessage>(this,
                (s, e) => RecordNavigation(e));
            WeakReferenceMessenger.Default.Register<ArtistViewNavigationRequestMessage>(this,
                (s, e) => RecordNavigation(e));
            WeakReferenceMessenger.Default.Register<HomeViewNavigationRequestMessage>(this,
                (s, e) => RecordNavigation(e));
            WeakReferenceMessenger.Default.Register<PlaylistViewNavigationRequestMessage>(this,
                (s, e) => RecordNavigation(e));
            WeakReferenceMessenger.Default.Register<PlaylistsViewNavigationRequestMessage>(this,
                (s, e) => RecordNavigation(e));
        }

        /// <summary>
        /// Gets whether or not the navigation can go backwards.
        /// </summary>
        public bool CanGoBack => _navigationStack is { Count: > 1 };

        /// <summary>
        /// Navigates backwards.
        /// </summary>
        public void NavigateBackwards()
        {
            if (!CanGoBack || _navigationStack == null)
                return;

            _navigationStack.Pop();
            var previous = _navigationStack.Peek();
            previous.RecordNavigation = false;

            switch (previous)
            {
                case AlbumViewNavigationRequestMessage albumViewNavigationRequest:
                    WeakReferenceMessenger.Default.Send(albumViewNavigationRequest);
                    break;
                case ArtistViewNavigationRequestMessage artistViewNavigationRequest:
                    WeakReferenceMessenger.Default.Send(artistViewNavigationRequest);
                    break;
                case HomeViewNavigationRequestMessage homeViewNavigationRequest:
                    WeakReferenceMessenger.Default.Send(homeViewNavigationRequest);
                    break;
                case PlaylistsViewNavigationRequestMessage playlistViewNavigationRequest:
                    WeakReferenceMessenger.Default.Send(playlistViewNavigationRequest);
                    break;
            }
        }

        private void RecordNavigation<T>(T viewModel)
            where T : PageNavigationRequestMessage
        {
            if (_navigationStack == null)
                return;

            if (viewModel.RecordNavigation)
                _navigationStack.Push(viewModel);
        }
    }
}
