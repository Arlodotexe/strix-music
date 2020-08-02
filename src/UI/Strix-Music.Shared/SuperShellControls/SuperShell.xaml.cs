using System;
using OwlCore.ArchTools;
using Strix_Music.SuperShellControls;
using StrixMusic.Services.SuperShell;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Strix_Music
{
    public sealed partial class SuperShell : UserControl
    {
        private LazyService<ISuperShellService> _superShellService;

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

            _superShellService.Value.VisibilityChanged += Value_VisibilityChanged;
        }

        private void Value_VisibilityChanged(object sender, SuperShellDisplays e)
        {
            switch (e)
            {
                case SuperShellDisplays.Hidden:
                    this.Visibility = Visibility.Collapsed;
                    break;
                case SuperShellDisplays.Settings:
                    this.Visibility = Visibility.Visible;
                    Presenter.Content = new SuperShellSettings();
                    break;
                case SuperShellDisplays.Debug:
                    throw new NotImplementedException();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            _superShellService.Value.Hide();
        }

        private void DetachEvents()
        {
            Unloaded -= SuperShell_Unloaded;

            _superShellService.Value.VisibilityChanged -= Value_VisibilityChanged;
        }
    }
}
