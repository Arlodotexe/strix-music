using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using LaunchPad.AbstractUI.ViewModels;
using OwlCore.AbstractUI.Models;

namespace LaunchPad.AbstractUI.Controls
{
    /// <summary>
    /// Displays a group of abstract UI elements.
    /// </summary>
    public sealed partial class AbstractUIGroupListPresenter : UserControl
    {
        /// <summary>
        /// The groups of abstract UI elements to display.
        /// </summary>
        private ObservableCollection<AbstractUIElementGroupViewModel> ViewModel
        {
            get => (ObservableCollection<AbstractUIElementGroupViewModel>)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        /// <summary>
        /// Backing property for <see cref="ViewModel"/>.
        /// </summary>
        private static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(nameof(AbstractUIElementGroup), typeof(ObservableCollection<AbstractUIElementGroupViewModel>), typeof(AbstractUIGroupListPresenter), new PropertyMetadata(new ObservableCollection<AbstractUIElementGroupViewModel>()));

        /// <summary>
        /// The template selector used to display Abstract UI elements. Use this to define your own custom styles for each control. You may specify the existing, default styles for those you don't want to override.
        /// </summary>
        public DataTemplateSelector TemplateSelector
        {
            get => (DataTemplateSelector)GetValue(TemplateSelectorProperty);
            set => SetValue(TemplateSelectorProperty, value);
        }

        /// <summary>
        /// Backing property for <see cref="TemplateSelector"/>.
        /// </summary>
        public static readonly DependencyProperty TemplateSelectorProperty =
            DependencyProperty.Register(nameof(TemplateSelector), typeof(DataTemplateSelector), typeof(AbstractUIGroupListPresenter), new PropertyMetadata(new AbstractUIGroupPresentationTemplateSelector()));

        /// <summary>
        /// Creates a new instance of <see cref="AbstractUIGroupListPresenter"/>.
        /// </summary>
        public AbstractUIGroupListPresenter()
        {
            this.InitializeComponent();

            Loaded += AbstractUIGroupListPresenter_Loaded;
        }

        private void AbstractUIGroupListPresenter_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.Clear();

            if (DataContext is IEnumerable<AbstractUIElementGroup> groups)
            {
                foreach (var group in groups)
                {
                    ViewModel.Add(new AbstractUIElementGroupViewModel(group, TemplateSelector));
                }
            }

            AttachEvents();
        }

        private void AbstractUIGroupListPresenter_Unloaded(object sender, RoutedEventArgs e)
        {
            DetachEvents();
        }

        private void AttachEvents()
        {
            Unloaded += AbstractUIGroupListPresenter_Unloaded;
            DataContextChanged += AbstractUIGroupListPresenter_DataContextChanged;
        }

        private void DetachEvents()
        {
            Unloaded -= AbstractUIGroupListPresenter_Unloaded;
            DataContextChanged -= AbstractUIGroupListPresenter_DataContextChanged;
        }

        private void AbstractUIGroupListPresenter_DataContextChanged(DependencyObject sender, DataContextChangedEventArgs args)
        {
            ViewModel.Clear();

            if (DataContext is IEnumerable<AbstractUIElementGroup> groups)
            {
                foreach(var group in groups)
                {
                    ViewModel.Add(new AbstractUIElementGroupViewModel(group, TemplateSelector));
                }
            }
        }
    }
}
