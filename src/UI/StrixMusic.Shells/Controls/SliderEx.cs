using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace StrixMusic.Shells.Controls
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
                typeof(ProgressSlider),
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
                return _isContainerHeld || _isThumbHeld;
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
                thumb.DragStarted += Thumb_DragStarted;
                thumb.DragCompleted += Thumb_DragCompleted;
                thumb.DragDelta += Thumb_DragDelta;
            }

            var sliderContainer = GetTemplateChild("SliderContainer") as Grid;
            if (sliderContainer != null)
            {
                sliderContainer.AddHandler(
                    PointerPressedEvent,
                    new PointerEventHandler(SliderContainer_PointerPressed),
                    true);

                sliderContainer.AddHandler(
                    PointerReleasedEvent,
                    new PointerEventHandler(SliderContainer_PointerReleased),
                    true);

                sliderContainer.AddHandler(
                    PointerMovedEvent,
                    new PointerEventHandler(SliderContainer_PointerMoved),
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
            InvokeMove();
        }

        private void SliderContainer_PointerReleased(
            object sender,
            PointerRoutedEventArgs e)
        {
            SetContainerHeld(false);
        }

        private void SliderContainer_PointerPressed(
            object sender,
            PointerRoutedEventArgs e)
        {
            SetContainerHeld(true);
        }

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            InvokeMove();
        }

        private void Thumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            SetThumbHeld(false);
        }

        private void Thumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            SetThumbHeld(true);
        }

        private void SetThumbHeld(bool held)
        {
            bool wasManipulated = IsSliderBeingManpulated;
            _isThumbHeld = held;
            InvokeStateChange(wasManipulated);
        }

        private void SetContainerHeld(bool held)
        {
            bool wasManipulated = IsSliderBeingManpulated;
            _isContainerHeld = held;
            InvokeStateChange(wasManipulated);
        }

        private void InvokeMove()
        {
            SliderManipulationMoved?.Invoke(this, EventArgs.Empty);
        }

        private void InvokeStateChange(bool wasBeingManipulated)
        {
            if (wasBeingManipulated != IsSliderBeingManpulated)
            {
                if (IsSliderBeingManpulated)
                {
                    SliderManipulationStarted?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    SliderManipulationCompleted?.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }
}
