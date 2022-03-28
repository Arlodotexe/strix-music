using OwlCore.AbstractUI.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace OwlCore.Uno.AbstractUI.Controls
{
    /// <summary>
    /// Displays a group of abstract UI elements.
    /// </summary>
    public sealed partial class AbstractUICollectionPresenter : Control
    {
        /// <summary>
        /// Creates a new instance of <see cref="AbstractUICollectionPresenter"/>.
        /// </summary>
        public AbstractUICollectionPresenter()
        {
            this.DefaultStyleKey = typeof(AbstractUICollectionPresenter);
        }

        /// <summary>
        /// Backing property for <see cref="ViewModel"/>.
        /// </summary>
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(nameof(ViewModel), typeof(AbstractUICollectionViewModel), typeof(AbstractUICollectionPresenter), new PropertyMetadata(null));

        /// <summary>
        /// Backing property for <see cref="TemplateSelector"/>.
        /// </summary>
        public static readonly DependencyProperty TemplateSelectorProperty =
            DependencyProperty.Register(nameof(TemplateSelector), typeof(DataTemplateSelector), typeof(AbstractUICollectionPresenter), new PropertyMetadata(null));

        /// <summary>
        /// The ViewModel for this UserControl.
        /// </summary>
        public AbstractUICollectionViewModel? ViewModel
        {
            get => (AbstractUICollectionViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        /// <summary>
        /// The template selector used to display Abstract UI elements. Use this to define your own custom styles for each control. You may specify the existing, default styles for those you don't want to override.
        /// </summary>
        public DataTemplateSelector? TemplateSelector
        {
            get => (DataTemplateSelector)GetValue(TemplateSelectorProperty);
            set => SetValue(TemplateSelectorProperty, value);
        }
    }
}
