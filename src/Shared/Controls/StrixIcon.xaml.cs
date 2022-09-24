using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Controls
{
    /// <summary>
    /// A control to display the Strix icon.
    /// </summary>
    public sealed partial class StrixIcon : UserControl
    {
        private bool _isAnimationEnding;

        /// <summary>
        /// The backing <see cref="DependencyProperty"/> for the <see cref="ShowText"/> property.
        /// </summary>
        public static readonly DependencyProperty ShowTextProperty =
            DependencyProperty.Register(nameof(ShowText), typeof(bool), typeof(StrixIcon), new PropertyMetadata(false));

        /// <summary>
        /// The backing <see cref="DependencyProperty"/> for the <see cref="IsAnimated"/> property.
        /// </summary>
        public static readonly DependencyProperty IsAnimatedProperty =
            DependencyProperty.Register(nameof(IsAnimated), typeof(bool), typeof(StrixIcon), new PropertyMetadata(false, (d, e) => d.Cast<StrixIcon>().OnAnimatedChanged()));

        /// <summary>
        /// The backing <see cref="DependencyProperty"/> for the <see cref="IsAnimated"/> property.
        /// </summary>
        public static readonly DependencyProperty PlayIntroProperty =
            DependencyProperty.Register(nameof(PlayIntroProperty), typeof(bool), typeof(StrixIcon), new PropertyMetadata(false));

        /// <summary>
        /// Initializes a new instance of the <see cref="StrixIcon"/> class.
        /// </summary>
        public StrixIcon()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Fired when the ending animation completes.
        /// </summary>
        public event EventHandler? AnimationFinished;

        /// <summary>
        /// Gets or sets a value indicating whether or not the text should show by the icon.
        /// </summary>
        public bool ShowText
        {
            get { return (bool)GetValue(ShowTextProperty); }
            set { SetValue(ShowTextProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the starting animation should play if animated.
        /// </summary>
        public bool PlayIntro
        {
            get { return (bool)GetValue(PlayIntroProperty); }
            set { SetValue(PlayIntroProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the icon should show by animated.
        /// </summary>
        public bool IsAnimated
        {
            get { return (bool)GetValue(IsAnimatedProperty); }
            set { SetValue(IsAnimatedProperty, value); }
        }

        /// <summary>
        /// A function that tells the control to begin ending the animation.
        /// </summary>
        public void FinishAnimation()
        {
            _isAnimationEnding = true;

            RepeatedAnimation.Stop();
            EndingAnimation.Begin();
        }

        private void OnAnimatedChanged()
        {
            if (IsAnimated)
            {
                if (PlayIntro)
                {
                    StartingAnimation.Begin();
                }
                else
                {
                    RepeatedAnimation.Begin();
                }
            }
            else
            {
                RepeatedAnimation.Stop();
            }
        }

        private void StartingAnimation_Completed(object sender, object e)
        {
            RepeatedAnimation.Begin();
        }

        private void RepeatedAnimation_Completed(object sender, object e)
        {
            if (!_isAnimationEnding)
                RepeatedAnimation.Begin();
        }

        private void EndingAnimation_Completed(object sender, object e)
        {
            AnimationFinished?.Invoke(this, EventArgs.Empty);
        }
    }
}
