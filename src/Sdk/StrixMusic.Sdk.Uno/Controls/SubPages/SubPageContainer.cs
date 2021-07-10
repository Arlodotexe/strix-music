using StrixMusic.Sdk.Services.Navigation;
using StrixMusic.Sdk.Uno.Controls.SubPages.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Sdk.Uno.Controls.SubPages
{
    /// <summary>
    /// A Templated <see cref="Control"/> for showing SubPages in a Shell.
    /// </summary>
    public abstract partial class SubPageContainer<T> : Control
    {
        private const string OpenState = "Open";
        private const string ClosedState = "Closed";
        private const string BackButtonState = "BackButton";
        private const string CloseButtonState = "CloseButton";
        private const string NormalState = "Normal";
        private const string FullScreenState = "FullScreen";

        private readonly Stack<T> _contentHistory;

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SX1309:Field names should begin with underscore", Justification = "Templating name")]
        private Button? PART_BackButton;
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SX1309:Field names should begin with underscore", Justification = "Templating name")]
        private TextBlock? PART_HeaderText;

        private INavigationService<T>? _navigationService;

        /// <summary>
        /// The backing <see cref="DependencyProperty"/> for the <see cref="BackButtonVisibility"/> property.
        /// </summary>
        public static readonly DependencyProperty BackButtonVisibilityProperty =
            DependencyProperty.Register(
            nameof(BackButtonVisibility),
            typeof(Visibility),
            typeof(SubPageContainer<T>),
            new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// The backing <see cref="DependencyProperty"/> for the <see cref="Content"/> property.
        /// </summary>
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register(
            nameof(Content),
            typeof(T),
            typeof(SubPageContainer<T>),
            new PropertyMetadata(null));

        /// <summary>
        /// The backing <see cref="DependencyProperty"/> for the <see cref="IsOpen"/> property.
        /// </summary>
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register(
            nameof(IsOpen),
            typeof(bool),
            typeof(SubPageContainer<T>),
            new PropertyMetadata(false));

        /// <summary>
        /// The backing <see cref="DependencyProperty"/> for the <see cref="Header"/> property.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(
            nameof(Header),
            typeof(string),
            typeof(SubPageContainer<T>),
            new PropertyMetadata(string.Empty));

        /// <summary>
        /// Initializes a new instance of the <see cref="SubPageContainer"/> class.
        /// </summary>
        public SubPageContainer()
        {
            this.DefaultStyleKey = typeof(SubPageContainer<T>);

            IsOpen = false;
            _contentHistory = new Stack<T>();
        }

        /// <summary>
        /// Fired when the <see cref="SubPageContainer{T}"/> opens.
        /// </summary>
        public event EventHandler? Opened;

        /// <summary>
        /// Fired when the <see cref="SubPageContainer{T}"/> closes.
        /// </summary>
        public event EventHandler? Closed;

        /// <inheritdoc/>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            PART_BackButton = (Button)GetTemplateChild(nameof(PART_BackButton));
            PART_HeaderText = (TextBlock)GetTemplateChild(nameof(PART_HeaderText));

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
        public T Content
        {
            get => (T)GetValue(ContentProperty);
            private set
            {
                SetValue(ContentProperty, value);
                UpdatePage(value);
            }
        }

        /// <summary>
        /// Gets the Header of the page displayed.
        /// </summary>
        public string Header
        {
            get => (string)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        /// <summary>
        /// Gets whether or not the SubPageContainer is open.
        /// </summary>
        public bool IsOpen
        {
            get => (bool)GetValue(IsOpenProperty);
            set
            {
                bool oldValue = (bool)GetValue(IsOpenProperty);
                if (value == oldValue)
                    return;

                SetValue(IsOpenProperty, value);

                if (value)
                {
                    VisualStateManager.GoToState(this, OpenState, true);
                    Opened?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    VisualStateManager.GoToState(this, ClosedState, true);
                    Closed?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Attaches the navigation service.
        /// </summary>
        public void AttachNavigationService(INavigationService<T> navigationService)
        {
            DetachNavigationService();
            _navigationService = navigationService;
            _navigationService.NavigationRequested += NavigationRequested;
            _navigationService.BackRequested += BackRequested;
        }

        /// <summary>
        /// Called when <see cref="Content"/> changes.
        /// </summary>
        /// <param name="page">The new <see cref="Content"/>.</param>
        protected virtual void UpdatePage(T page)
        {
            if (page is ISubPage iPage)
            {
                Header = iPage.Header;
            }

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

        private void AttachHandlers()
        {
            this.Unloaded += SubPageContainer_Unloaded;

            if (PART_BackButton != null)
            {
                PART_BackButton.Click += BackButton_Clicked;
            }
        }

        private void DetachHandlers()
        {
            this.Unloaded -= SubPageContainer_Unloaded;

            if (PART_BackButton != null)
            {
                PART_BackButton.Click -= BackButton_Clicked;
            }

            DetachNavigationService();
        }

        private void DetachNavigationService()
        {
            if (_navigationService != null)
            {
                _navigationService.NavigationRequested -= NavigationRequested;
                _navigationService.BackRequested -= BackRequested;
            }
        }

        private void SubPageContainer_Unloaded(object sender, RoutedEventArgs e)
        {
            DetachHandlers();
        }

        private void NavigationRequested(object sender, NavigateEventArgs<T> e)
        {
            if (!e.IsOverlay)
            {
                if (!IsOpen)
                    return;

                // TODO: Close overlay and don't handle navigation
                _contentHistory.Clear();
                UpdateBackButtonState();
                return;
            }

            if (IsOpen)
            {
                _contentHistory.Push(Content);
                UpdateBackButtonState();
            }

            IsOpen = true;
            Content = e.Page;
        }

        private void BackRequested(object sender, System.EventArgs e)
        {
            if (!IsOpen)
                return;

            if (_contentHistory.Count > 0)
            {
                Content = _contentHistory.Pop();
                UpdateBackButtonState();
            } else
            {
                IsOpen = false;
            }
        }

        private void UpdateBackButtonState()
        {
            if (_contentHistory.Count > 0)
                VisualStateManager.GoToState(this, BackButtonState, true);
            else
                VisualStateManager.GoToState(this, CloseButtonState, true);
        }

        private void BackButton_Clicked(object sender, RoutedEventArgs e)
        {
            if (_navigationService != null)
                _navigationService.GoBack();
        }
    }
}
