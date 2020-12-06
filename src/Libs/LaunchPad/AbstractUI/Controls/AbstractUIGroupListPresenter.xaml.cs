using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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
        public List<AbstractUIElementGroup> ViewModel
        {
            get => (List<AbstractUIElementGroup>)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        /// <summary>
        /// Backing property for <see cref="ViewModel"/>.
        /// </summary>
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(nameof(AbstractUIElementGroup), typeof(List<AbstractUIElementGroup>), typeof(AbstractUIGroupListPresenter), new PropertyMetadata(new List<AbstractUIGroupListPresenter>()));

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
            DependencyProperty.Register(nameof(TemplateSelector), typeof(DataTemplateSelector), typeof(AbstractUIGroupListPresenter), new PropertyMetadata(0));

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
            if (DataContext is List<AbstractUIElementGroup> groups)
            {
                ViewModel = groups;
            }
        }
    }
}
