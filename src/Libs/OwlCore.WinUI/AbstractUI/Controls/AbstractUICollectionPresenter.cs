using OwlCore.AbstractUI.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace OwlCore.WinUI.AbstractUI.Controls
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
        /// The ViewModel for this UserControl.
        /// </summary>
        public AbstractUICollectionViewModel? ViewModel
        {
            get => (AbstractUICollectionViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }
    }
}
