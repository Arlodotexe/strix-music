using System;
using OwlCore.Remoting.Attributes;

namespace OwlCore.AbstractUI.Models
{
    /// <summary>
    /// Represents a UI element that displays a progress state. (ProgressBar, ProgressRing)
    /// </summary>
    public class AbstractProgressUIElement : AbstractUIElement
    {
        private double _minimum;
        private double _maximum;
        private double? _value;

        /// <summary>
        /// Creates a new instance of <see cref="AbstractProgressUIElement"/>.
        /// </summary>
        /// <param name="id"><inheritdoc cref="AbstractUIBase.Id"/></param>
        /// <param name="val">The value of the progess.</param>
        /// <param name="max">The maximum value of the progess.</param>
        /// <param name="min">The minimum value of the progess.</param>
        public AbstractProgressUIElement(string id, double? val, double max = 100, double min = 0)
            : base(id)
        {
            _value = val;
            _maximum = max;
            _minimum = min;
        }

        /// <summary>
        /// Fires when the <see cref="Value"/> changes.
        /// </summary>
        public event EventHandler<double?>? ValueChanged;

        /// <summary>
        /// Fires when the <see cref="Maximum"/> changes.
        /// </summary>
        public event EventHandler<double>? MaximumChanged;

        /// <summary>
        /// Fires when the <see cref="Minimum"/> changes.
        /// </summary>
        public event EventHandler<double>? MinimumChanged;

        /// <summary>
        /// Gets or sets the value for the progress to be.
        /// </summary>
        /// <remarks>
        /// Value is null if indeterminate.
        /// </remarks>
        [RemoteProperty]
        public double? Value
        {
            get => _value;
            set
            {
                _value = value;
                ValueChanged?.Invoke(this, value);
            }
        }

        /// <summary>
        /// Gets or sets the maximum value for the progress to be.
        /// </summary>
        [RemoteProperty]
        public double Maximum
        {
            get => _maximum;
            set
            {
                _maximum = value;
                MaximumChanged?.Invoke(this, value);
            }
        }

        /// <summary>
        /// Gets or sets the minimum value for the progress to be.
        /// </summary>
        [RemoteProperty]
        public double Minimum
        {
            get => _minimum;
            set
            {
                _minimum = value;
                MinimumChanged?.Invoke(this, value);
            }
        }
    }
}
