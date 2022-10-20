using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Sdk.WinUI.Controls;
using StrixMusic.Sdk.WinUI.Globalization;
using StrixMusic.Shells.Groove.Helper;
using StrixMusic.Shells.Groove.Messages.Navigation.Pages;
using StrixMusic.Shells.Groove.Messages.Navigation.Pages.Abstract;
using StrixMusic.Shells.Groove.ViewModels.Collections;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace StrixMusic.Shells.Groove
{
    public sealed partial class GrooveMusic : Shell
    {
        private readonly NavigationTracker _navigationTracker = new();

        /// <summary>
        /// A backing <see cref="DependencyProperty"/> for the <see cref="Title"/> property.
        /// </summary>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(GrooveMusic), new PropertyMetadata(string.Empty));

        /// <summary>
        /// A backing <see cref="DependencyProperty"/> for the <see cref="ShowLargeHeader"/> property.
        /// </summary>
        public static readonly DependencyProperty ShowLargeHeaderProperty =
            DependencyProperty.Register(nameof(ShowLargeHeader), typeof(bool), typeof(GrooveMusic), new PropertyMetadata(true));

        /// <summary>
        /// A backing <see cref="DependencyProperty"/> for the <see cref="PlaylistCollectionViewModel"/> property.
        /// </summary>
        public static readonly DependencyProperty PlaylistCollectionViewModelProperty =
            DependencyProperty.Register(nameof(PlaylistCollectionViewModel), typeof(GroovePlaylistCollectionViewModel), typeof(GrooveMusic), new PropertyMetadata(null));
        
        /// <summary>
        /// A backing <see cref="DependencyProperty"/> for the <see cref="Title"/> property.
        /// </summary>
        public static readonly DependencyProperty HamburgerPressedCommandProperty =
            DependencyProperty.Register(nameof(HamburgerPressedCommand), typeof(RelayCommand), typeof(GrooveMusic), new PropertyMetadata(null));

        /// <summary>
        /// Initializes a new instance of the <see cref="GrooveMusic"/> class.
        /// </summary>
        public GrooveMusic()
        {
            this.InitializeComponent();

            // Register home page navigation
            WeakReferenceMessenger.Default.Register<HomeViewNavigationRequestMessage>(this, (s, e) => NavigatePage(e));

            // Register album, artist, and playlist page navigation
            WeakReferenceMessenger.Default.Register<AlbumViewNavigationRequestMessage>(this, (s, e) => NavigatePage(e));
            WeakReferenceMessenger.Default.Register<ArtistViewNavigationRequestMessage>(this, (s, e) => NavigatePage(e));
            WeakReferenceMessenger.Default.Register<PlaylistViewNavigationRequestMessage>(this, (s, e) => NavigatePage(e));

            // Register playlists page navigation
            WeakReferenceMessenger.Default.Register<PlaylistsViewNavigationRequestMessage>(this, (s, e) => NavigatePage(e));

            HamburgerPressedCommand = new RelayCommand(HamburgerToggled);

            RegisterPropertyChangedCallback(RootProperty, (x, _) => ((GrooveMusic)x).OnRootChanged());

            Unloaded += GrooveShell_Unloaded;
            Loaded += GrooveShell_Loaded;
        }

        private void GrooveShell_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= GrooveShell_Loaded;

            _navigationTracker.Initialize();
            OnRootChanged();
        }

        private void GrooveShell_Unloaded(object sender, RoutedEventArgs e)
        {
            Unloaded -= GrooveShell_Unloaded;

            WeakReferenceMessenger.Default.Reset();
        }

        private void OnRootChanged()
        {
            if (Root is null)
                return;

            var libVm = Root.Library as LibraryViewModel ?? new LibraryViewModel(Root.Library, Root);

            PlaylistCollectionViewModel = new GroovePlaylistCollectionViewModel
            {
                PlaylistCollection = libVm,
            };

            if (Root?.Library != null)
            {
                _ = WeakReferenceMessenger.Default.Send(new HomeViewNavigationRequestMessage(libVm));
            }
        }

        /// <summary>
        /// Gets or sets the Title text for the Groove Shell.
        /// </summary>
        public bool ShowLargeHeader
        {
            get { return (bool)GetValue(ShowLargeHeaderProperty); }
            set { SetValue(ShowLargeHeaderProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Title text for the Groove Shell.
        /// </summary>
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        /// <summary>
        /// The <see cref="GroovePlaylistCollectionViewModel"/> for the <see cref="Controls.Collections.GroovePlaylistCollection"/> on display in the pane.
        /// </summary>
        public GroovePlaylistCollectionViewModel? PlaylistCollectionViewModel
        {
            get => (GroovePlaylistCollectionViewModel?)GetValue(PlaylistCollectionViewModelProperty);
            set => SetValue(PlaylistCollectionViewModelProperty, value);
        }

        /// <summary>
        /// Gets a Command that handles a Hamburger button press.
        /// </summary>
        public RelayCommand HamburgerPressedCommand
        {
            get => (RelayCommand)GetValue(HamburgerPressedCommandProperty);
            set => SetValue(HamburgerPressedCommandProperty, value);
        }

        /// <inheritdoc/>
        protected override void SetupTitleBar()
        {
            base.SetupTitleBar();

#if NETFX_CORE
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonForegroundColor = Colors.White;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
#endif

            SystemNavigationManager currentView = SystemNavigationManager.GetForCurrentView();
            currentView.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            currentView.BackRequested += CurrentView_BackRequested;
        }

        private void CurrentView_BackRequested(object sender, BackRequestedEventArgs e)
        {
            _navigationTracker.NavigateBackwards();
        }

        private void NavigationButtonClicked(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton button)
            {
                switch (button.Tag as string)
                {
                    case "MyMusic":
                        Guard.IsNotNull(Root?.Library, nameof(Root.Library));
                        WeakReferenceMessenger.Default.Send(new HomeViewNavigationRequestMessage((LibraryViewModel)Root.Library));
                        break;
                    case "Playlists":
                        Guard.IsNotNull(Root?.Library, nameof(Root.Library));
                        WeakReferenceMessenger.Default.Send(new PlaylistsViewNavigationRequestMessage((LibraryViewModel)Root.Library));
                        break;
                }
            }
        }

        private void HamburgerToggled()
        {
            MainSplitView.IsPaneOpen = !MainSplitView.IsPaneOpen;
        }

        private void OnPaneStateChanged(SplitView sender, object e) => UpdatePaneState();

        private void UpdatePaneState()
        {
            if (MainSplitView.IsPaneOpen)
                VisualStateManager.GoToState(this, "Full", true);
            else
                VisualStateManager.GoToState(this, "Compact", true);
        }

        private void NavigatePage<T>(PageNavigationRequestMessage<T> viewModel)
        {
            MainContent.Content = viewModel.PageData;

            if (viewModel is PlaylistsViewNavigationRequestMessage playlistsNavReq)
            {
                if (Resources.TryGetValue("GroovePlaylistsPageDataTemplate", out var dataTemplate))
                    MainContent.ContentTemplate = (DataTemplate)dataTemplate;
            }
            
            Title = LocalizationResources.Music?.GetString(viewModel.PageTitleResource) ?? viewModel.PageTitleResource;
            ShowLargeHeader = viewModel.ShowLargeHeader;

            UpdateSelectedNavigationButton(viewModel);
        }

        private void UpdateSelectedNavigationButton<T>(PageNavigationRequestMessage<T> viewModel)
        {
            ToggleButton? button = viewModel switch
            {
                HomeViewNavigationRequestMessage _ => MyMusicButton,
                PlaylistViewNavigationRequestMessage _ => PlaylistsButton,
                _ => null,
            };

            // Reset all buttons, but not the PlaylistList
            MyMusicButton.IsChecked = false;
            RecentButton.IsChecked = false;
            NowPlayingButton.IsChecked = false;
            PlaylistsButton.IsChecked = false;

            if (button != null)
            {
                PlaylistList.ClearSelected();

                // Set the active navigation button
                button.IsChecked = true;
            }
        }
    }
}
