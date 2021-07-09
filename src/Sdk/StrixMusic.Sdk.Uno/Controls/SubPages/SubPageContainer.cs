using Microsoft.Toolkit.Mvvm.DependencyInjection;
using StrixMusic.Sdk.Services.Navigation;
using StrixMusic.Sdk.Uno.Controls.SubPages.Types;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Sdk.Uno.Controls.SubPages
{
    /// <summary>
    /// A Templated <see cref="Control"/> for showing SubPages in a Shell.
    /// </summary>
    public partial class SubPageContainer : Control
    {
        private const string OpenState = "Open";
        private const string ClosedState = "Closed";
        private const string BackButtonState = "BackButton";
        private const string CloseButtonState = "CloseButton";
        private const string NormalState = "Normal";
        private const string FullScreenState = "FullScreen";

        private NavigationService<object> _navigationService;
        private Stack<object> _contentHistory;
        private bool _isOpen;

        /// <summary>
        /// The backing <see cref="DependencyProperty"/> for the <see cref="BackButtonVisibility"/> property.
        /// </summary>
        public static readonly DependencyProperty BackButtonVisibilityProperty =
            DependencyProperty.Register(
            nameof(BackButtonVisibility),
            typeof(Visibility),
            typeof(SubPageContainer),
            new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// The backing <see cref="DependencyProperty"/> for the <see cref="Content"/> property.
        /// </summary>
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register(
            nameof(Content),
            typeof(object),
            typeof(SubPageContainer),
            new PropertyMetadata(null));

        /// <summary>
        /// The backing <see cref="DependencyProperty"/> for the <see cref="IsOpen"/> property.
        /// </summary>
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register(
            nameof(IsOpen),
            typeof(bool),
            typeof(SubPageContainer),
            new PropertyMetadata(false));

        /// <summary>
        /// Initializes a new instance of the <see cref="SubPageContainer"/> class.
        /// </summary>
        public SubPageContainer()
        {
            this.DefaultStyleKey = typeof(SubPageContainer);

            _isOpen = false;
            _navigationService = Ioc.Default.GetRequiredService<NavigationService<object>>();
            _contentHistory = new Stack<object>();
        }

        /// <inheritdoc/>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            AttachHandlers();
        }

        /// <summary>
        /// Gets or sets the <see cref="Visibility"/> of the BackButton.
        /// </summary>
        public Visibility BackButtonVisibility
        {
            get => (Visibility)GetValue(BackButtonVisibilityProperty);
            set => SetValue(BackButtonVisibilityProperty, value);
        }

        /// <summary>
        /// Gets the Content on Display by the template's ContentControl.
        /// </summary>
        public object Content
        {
            get => GetValue(ContentProperty);
            private set
            {
                SetValue(ContentProperty, value);
                UpdatePage(value);
            }
        }

        /// <summary>
        /// Gets whether or not the SubPageContainer is open.
        /// </summary>
        public bool IsOpen
        {
            get => (bool)GetValue(IsOpenProperty);
            set
            {
                SetValue(IsOpenProperty, value);
                if (value) VisualStateManager.GoToState(this, OpenState, true);
                else VisualStateManager.GoToState(this, ClosedState, true);
            }
        }

        private void AttachHandlers()
        {
            this.Unloaded += SubPageContainer_Unloaded;

            _navigationService.NavigationRequested += NavigationRequested;
            _navigationService.BackRequested += BackRequested;
        }

        private void DetachHandlers()
        {
            this.Unloaded -= SubPageContainer_Unloaded;
        }

        private void SubPageContainer_Unloaded(object sender, RoutedEventArgs e)
        {
            DetachHandlers();
        }

        private void NavigationRequested(object sender, NavigateEventArgs<object> e)
        {
            if (!e.IsOverlay)
            {
                if (!_isOpen)
                    return;

                // TODO: Close overlay and don't handle navigation
                _contentHistory.Clear();
                UpdateBackButtonState();
                return;
            }

            if (_isOpen)
            {
                _contentHistory.Push(Content);
                UpdateBackButtonState();
            }

            _isOpen = true;
            Content = e.Page;
        }

        private void BackRequested(object sender, System.EventArgs e)
        {
            if (!_isOpen)
                return;

            if (_contentHistory.Count > 0)
            {
                Content = _contentHistory.Pop();
                UpdateBackButtonState();
            } else
            {
                // TODO: Close overlay
            }
        }

        private void UpdatePage(object page)
        {
            switch (page)
            {
                case IFullScreenPage _:
                    VisualStateManager.GoToState(this, FullScreenState, true);
                    break;
                default:
                    VisualStateManager.GoToState(this, NormalState, true);
                    break;
            }
        }

        private void UpdateBackButtonState()
        {
            if (_contentHistory.Count > 0)
                VisualStateManager.GoToState(this, BackButtonState, true);
            else
                VisualStateManager.GoToState(this, CloseButtonState, true);
        }
    }
}
