using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace StrixMusic.Shell.Default.Controls
{
    /// <summary>
    /// An extended <see cref="Slider"/> that has events for the Slider is modifed by the user.
    /// </summary>
    /// <remarks>
    /// Partially stolen from StackOverflow:
    /// https://stackoverflow.com/questions/48833441/how-do-i-listen-to-uwp-xaml-slider-manipulation-start-end-events
    /// </remarks>
    public partial class SliderEx : Slider
    {
        /// <summary>
        /// <see cref="DependencyProperty"/> for the <see cref="Remaining"/> property.
        /// </summary>
        public static readonly DependencyProperty RemainingProperty =
            DependencyProperty.Register(
                nameof(Remaining),
                typeof(bool),
                typeof(ProgressSliderControl),
                new PropertyMetadata(true));

        /// <summary>
        /// The <see cref="Slider"/> has begun being manipulation by the user.
        /// </summary>
        public event EventHandler? SliderManipulationStarted;

        /// <summary>
        /// The <see cref="Slider"/> has finished being manipulation by user.
        /// </summary>
        public event EventHandler? SliderManipulationCompleted;

        /// <summary>
        /// The <see cref="Slider"/> has value has changed as a result of the user.
        /// </summary>
        public event EventHandler? SliderManipulationMoved;

        /// <summary>
        /// Gets the value remaining on the Slider.
        /// </summary>
        public double Remaining
        {
            get => (double)GetValue(RemainingProperty);
            private set => SetValue(RemainingProperty, value);
        }

        private bool IsSliderBeingManpulated
        {
            get
            {
                return this._isContainerHeld || this._isThumbHeld;
            }
        }

        private bool _isThumbHeld = false;
        private bool _isContainerHeld = false;

        /// <inheritdoc/>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var thumb = GetTemplateChild("HorizontalThumb") as Thumb;
            if (thumb == null)
            {
                thumb = GetTemplateChild("VerticalThumb") as Thumb;
            }

            if (thumb != null)
            {
                thumb.DragStarted += this.Thumb_DragStarted;
                thumb.DragCompleted += this.Thumb_DragCompleted;
                thumb.DragDelta += this.Thumb_DragDelta;
            }

            var sliderContainer = GetTemplateChild("SliderContainer") as Grid;
            if (sliderContainer != null)
            {
                sliderContainer.AddHandler(
                    PointerPressedEvent,
                    new PointerEventHandler(this.SliderContainer_PointerPressed),
                    true);

                sliderContainer.AddHandler(
                    PointerReleasedEvent,
                    new PointerEventHandler(this.SliderContainer_PointerReleased),
                    true);

                sliderContainer.AddHandler(
                    PointerMovedEvent,
                    new PointerEventHandler(this.SliderContainer_PointerMoved),
                    true);
            }
        }

        /// <inheritdoc/>
        protected override void OnValueChanged(double oldValue, double newValue)
        {
            base.OnValueChanged(oldValue, newValue);
            Remaining = Maximum - newValue;
        }

        private void SliderContainer_PointerMoved(
            object sender,
            PointerRoutedEventArgs e)
        {
            this.InvokeMove();
        }

        private void SliderContainer_PointerReleased(
            object sender,
            PointerRoutedEventArgs e)
        {
            this.SetContainerHeld(false);
        }

        private void SliderContainer_PointerPressed(
            object sender,
            PointerRoutedEventArgs e)
        {
            this.SetContainerHeld(true);
        }

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            this.InvokeMove();
        }

        private void Thumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            this.SetThumbHeld(false);
        }

        private void Thumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            this.SetThumbHeld(true);
        }

        private void SetThumbHeld(bool held)
        {
            bool wasManipulated = this.IsSliderBeingManpulated;
            this._isThumbHeld = held;
            this.InvokeStateChange(wasManipulated);
        }

        private void SetContainerHeld(bool held)
        {
            bool wasManipulated = this.IsSliderBeingManpulated;
            this._isContainerHeld = held;
            this.InvokeStateChange(wasManipulated);
        }

        private void InvokeMove()
        {
            this.SliderManipulationMoved?.Invoke(this, EventArgs.Empty);
        }

        private void InvokeStateChange(bool wasBeingManipulated)
        {
            if (wasBeingManipulated != this.IsSliderBeingManpulated)
            {
                if (this.IsSliderBeingManpulated)
                {
                    this.SliderManipulationStarted?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    this.SliderManipulationCompleted?.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }
}
