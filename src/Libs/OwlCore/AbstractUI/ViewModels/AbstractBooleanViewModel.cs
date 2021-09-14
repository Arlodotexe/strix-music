using Microsoft.Toolkit.Mvvm.Input;
using OwlCore.AbstractUI.Models;

namespace OwlCore.AbstractUI.ViewModels
{
    /// <summary>
    /// Contains bindable information about an <see cref="AbstractBoolean"/>.
    /// </summary>
    public class AbstractBooleanViewModel : AbstractUIViewModelBase
    {
        private string _label;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractBooleanViewModel"/> class.
        /// </summary>
        /// <param name="model">The model to wrap around.</param>
        public AbstractBooleanViewModel(AbstractBoolean model)
            : base(model)
        {
            ToggledCommand = new RelayCommand(OnToggled);
            _label = model.Label ?? string.Empty;

            AttachEvents(model);
        }

        private void AttachEvents(AbstractBoolean model)
        {
            model.StateChanged += Model_StateChanged;
            model.LabelChanged += Model_LabelChanged;
        }

        private void DetachEvents(AbstractBoolean model)
        {
            model.StateChanged -= Model_StateChanged;
            model.LabelChanged -= Model_LabelChanged;
        }

        private void OnToggled()
        {
            ((AbstractBoolean)Model).State = IsToggled;
        }

        private void Model_LabelChanged(object sender, string e) => Label = e;

        private void Model_StateChanged(object sender, bool e) => IsToggled = e;

        /// <inheritdoc cref="AbstractBoolean.Label"/>
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
            get => ((AbstractBoolean)Model).State;
            set
            {
                if (value == ((AbstractBoolean)Model).State)
                    return;

                ((AbstractBoolean)Model).State = value;
            }
        }

        /// <summary>
        /// Run this command when the user toggles the UI element.
        /// </summary>
        public IRelayCommand ToggledCommand { get; }

        /// <inheritdoc/>
        public override void Dispose()
        {
            DetachEvents((AbstractBoolean)Model);
            base.Dispose();
        }
    }
}
