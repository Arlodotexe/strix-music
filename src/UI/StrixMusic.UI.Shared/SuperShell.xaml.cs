using System.Collections.ObjectModel;
using LaunchPad.AbstractUI.Controls;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using OwlCore.AbstractUI.Models;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Services.Settings;
using StrixMusic.Sdk.Uno.Services;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shared
{
    /// <summary>
    /// The SuperShell is an in-app overlay that sits on top of all other shells. This provides a way for the user to change settings even if the current shell has a catastrophic failure.
    /// </summary>
    public sealed partial class SuperShell : UserControl
    {
        /// <summary>
        /// The groups of items shown in the UI.
        /// </summary>
        public ObservableCollection<UIElement> Items { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SuperShell"/> class.
        /// </summary>
        public SuperShell()
        {
            InitializeComponent();

            Items = new ObservableCollection<UIElement>()
            {
            };

            PART_Pivot.Items.Add(new AbstractUIGroupListPresenter()
            {
                DataContext = new DefaultAbstractUISettings().AllElementGroups,
            });

            Loaded += SuperShell_Loaded;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SuperShell"/> class, and displays a core's <see cref="AbstractUIElement"/>s.
        /// </summary>
        public SuperShell(ICore core)
            : this()
        {
            PART_Pivot.Items.Add(new AbstractUIGroupListPresenter()
            {
                DataContext = core.CoreConfig.AbstractUIElements,
            });
        }

        private async void SuperShell_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= SuperShell_Loaded;
        }
    }
}
