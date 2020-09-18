using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using StrixMusic.SuperShellControls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using StrixMusic.Sdk.Services.SuperShell;

namespace StrixMusic
{
    public sealed partial class SuperShell : UserControl
    {
        private readonly ISuperShellService _superShellService = Ioc.Default.GetService<ISuperShellService>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SuperShell"/> class.
        /// </summary>
        public SuperShell()
        {
            this.InitializeComponent();

            Loaded += SuperShell_Loaded;
        }

        private void SuperShell_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= SuperShell_Loaded;

            AttachEvents();
        }

        private void SuperShell_Unloaded(object sender, RoutedEventArgs e)
        {
            DetachEvents();
        }

        private void AttachEvents()
        {
            Unloaded += SuperShell_Unloaded;

            _superShellService.VisibilityChanged += Value_VisibilityChanged;
        }

        private void Value_VisibilityChanged(object sender, SuperShellDisplayState e)
        {
            switch (e)
            {
                case SuperShellDisplayState.Hidden:
                    this.Visibility = Visibility.Collapsed;
                    break;
                case SuperShellDisplayState.Settings:
                    this.Visibility = Visibility.Visible;
                    Presenter.Content = new SuperShellSettings();
                    break;
                case SuperShellDisplayState.Debug:
                    throw new NotImplementedException();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            _superShellService.Hide();
        }

        private void DetachEvents()
        {
            Unloaded -= SuperShell_Unloaded;

            _superShellService.VisibilityChanged -= Value_VisibilityChanged;
        }
    }
}
