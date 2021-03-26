using Microsoft.Toolkit.Mvvm.Input;
using OwlCore.AbstractUI.Models;
using System.ComponentModel;

namespace OwlCore.AbstractUI.ViewModels
{
    /// <summary>
    /// Contains bindable information about an <see cref="AbstractBooleanUIElement"/>.
    /// </summary>
    [Bindable(true)]
    public class AbstractBooleanViewModel : AbstractUIViewModelBase
    {
        private bool _isToggled;
        private string _label;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractBooleanViewModel"/> class.
        /// </summary>
        /// <param name="model">The model to wrap around.</param>
        public AbstractBooleanViewModel(AbstractBooleanUIElement model)
            : base(model)
        {
            ToggledCommand = new RelayCommand(OnToggled);
            _label = model.Label ?? string.Empty;
            _isToggled = model.State;

            AttachEvents(model);
        }

        private void AttachEvents(AbstractBooleanUIElement model)
        {
            model.StateChanged += Model_StateChanged;
            model.LabelChanged += Model_LabelChanged;
        }

        private void DetachEvents(AbstractBooleanUIElement model)
        {
            model.StateChanged -= Model_StateChanged;
            model.LabelChanged -= Model_LabelChanged;
        }

        private void OnToggled()
        {
            ((AbstractBooleanUIElement)Model).ChangeState(IsToggled);
        }

        private void Model_LabelChanged(object sender, string e) => Label = e;

        private void Model_StateChanged(object sender, bool e) => IsToggled = e;

        /// <inheritdoc cref="AbstractBooleanUIElement.Label"/>
        public string Label
        {
            get => _label;
            set => SetProperty(ref _label, value);
        }

        /// <summary>
        /// Indicates if the UI element is in a toggled state.
        /// </summary>
        public bool IsToggled
        {
            get => _isToggled;
            set => SetProperty(ref _isToggled, value);
        }

        /// <summary>
        /// Run this command when the user toggles the UI element.
        /// </summary>
        public IRelayCommand ToggledCommand { get; }
    }
}
