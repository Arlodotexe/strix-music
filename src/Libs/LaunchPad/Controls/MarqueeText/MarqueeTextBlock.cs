/// https://github.com/sanje2v/MarqueeTextControl/blob/master/MarqueeText/MarqueeTextControl.cs
/// Heavily modfied Control created by sanje2v

using System;
using System.Linq;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace LaunchPad.Controls
{
    /// <summary>
    /// A Control that displays Text in a Marquee style.
    /// </summary>
    [TemplatePart(Name = "canvas", Type = typeof(Canvas))]
    [TemplatePart(Name = "stackpanel", Type = typeof(StackPanel))]
    [TemplatePart(Name = "textblock", Type = typeof(TextBlock))]
    [TemplatePart(Name = "textblock2", Type = typeof(TextBlock))]
    [ContentProperty(Name = "Text")]
    public sealed partial class MarqueeTextBlock : Control
    {
        private static string CANVAS_NAME = "canvas";
        private static string RECT_GEOMETRY_CANVAS_NAME = "rectanglegeometeryClipCanvas";
        private static string TEXTBLOCK_NAME = "textblock";
        private static string TEXTBLOCK2_NAME = "textblock2";
        private static string STACKPANEL_NAME = "stackpanel";

        private static string VISUALSTATE_STOPPED = "visualstateStopped";
        private static string VISUALSTATE_MARQUEE = "visualstateMarquee";
        private Storyboard? _storyboardMarquee;

        private static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(MarqueeTextBlock), new PropertyMetadata(null,
            (sender, e) =>
            {
                var control = (MarqueeTextBlock)sender;

                control.StartMarqueeAnimationIfNeeded();
            }));

        private static readonly DependencyProperty IsStoppedProperty = DependencyProperty.Register(nameof(IsStopped), typeof(bool), typeof(MarqueeTextBlock), new PropertyMetadata(false,
            (sender, e) =>
            {
                var control = (MarqueeTextBlock)sender;

                control.StartMarqueeAnimationIfNeeded();
            }));

        private static readonly DependencyProperty EasingFunctionProperty = DependencyProperty.Register(nameof(EasingFunction), typeof(EasingFunctionBase), typeof(MarqueeTextBlock), new PropertyMetadata(null,
            (sender, e) =>
            {
                var control = (MarqueeTextBlock)sender;

                control.StartMarqueeAnimationIfNeeded();
            }));

        private static readonly DependencyProperty IsAbsoluteSpeedProperty = DependencyProperty.Register(nameof(IsAbsoluteSpeed), typeof(bool), typeof(MarqueeTextBlock), new PropertyMetadata(false,
            (sender, e) =>
            {
                var control = (MarqueeTextBlock)sender;

                control.StartMarqueeAnimationIfNeeded();
            }));

        private static readonly DependencyProperty AbsoluteSpeedProperty = DependencyProperty.Register(nameof(AbsoluteSpeed), typeof(double), typeof(MarqueeTextBlock), new PropertyMetadata(32d,
            (sender, e) =>
            {
                var control = (MarqueeTextBlock)sender;

                control.StartMarqueeAnimationIfNeeded();
            }));

        private static readonly DependencyProperty AnimationDurationProperty = DependencyProperty.Register(nameof(AnimationDuration), typeof(TimeSpan), typeof(MarqueeTextBlock), new PropertyMetadata(TimeSpan.FromSeconds(4),
            (sender, e) =>
            {
                var control = (MarqueeTextBlock)sender;

                control.StartMarqueeAnimationIfNeeded();
            }));

        private static readonly DependencyProperty IsTickerProperty = DependencyProperty.Register(nameof(IsTicker), typeof(bool), typeof(MarqueeTextBlock), new PropertyMetadata(false,
            (sender, e) =>
            {
                var control = (MarqueeTextBlock)sender;

                control.StartMarqueeAnimationIfNeeded();
            }));

        //private static readonly DependencyProperty AnimationSpeedRatioProperty = DependencyProperty.Register(nameof(AnimationSpeedRatio), typeof(double), typeof(MarqueeTextControl), new PropertyMetadata(1.0d,
        //    (sender, e) =>
        //    {
        //        var control = (MarqueeTextControl)sender;

        //        control.StartMarqueeAnimationIfNeeded();
        //    }));

        private static readonly DependencyProperty IsRepeatingProperty = DependencyProperty.Register(nameof(IsRepeating), typeof(bool), typeof(MarqueeTextBlock), new PropertyMetadata(true,
            (sender, e) =>
            {
                var control = (MarqueeTextBlock)sender;

                control.StartMarqueeAnimationIfNeeded();
            }));

        /// <summary>
        /// Initializes a new instance of the <see cref="MarqueeTextBlock"/> class.
        /// </summary>
        public MarqueeTextBlock()
            : base()
        {
            this.DefaultStyleKey = typeof(MarqueeTextBlock);

            FontSize_OnChanged(this, FontSizeProperty);
            RegisterPropertyChangedCallback(FontSizeProperty, FontSize_OnChanged);
        }

        void FontSize_OnChanged(DependencyObject sender, DependencyProperty dp)
        {
            // Set minimum height
            var displayInfo = DisplayInformation.GetForCurrentView();
            var minimumHeight = ((this.FontSize / 72.0d) * displayInfo.LogicalDpi) / displayInfo.RawPixelsPerViewPixel;
            this.MinHeight = minimumHeight;
        }

        /// <summary>
        /// Gets or sets the text being displayed in Marquee.
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether or not the Marquee is stopped.
        /// </summary>
        public bool IsStopped
        {
            get { return (bool)GetValue(IsStoppedProperty); }
            set { SetValue(IsStoppedProperty, value); }
        }

        /// <summary>
        /// Gets or sets the easing of the animation rate.
        /// </summary>
        public EasingFunctionBase EasingFunction
        {
            get { return (EasingFunctionBase)GetValue(EasingFunctionProperty); }
            set { SetValue(EasingFunctionProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether or not the speed of the text is absolute or frequency based.
        /// </summary>
        public bool IsAbsoluteSpeed
        {
            get { return (bool)GetValue(IsAbsoluteSpeedProperty); }
            set { SetValue(IsAbsoluteSpeedProperty, value); }
        }

        /// <summary>
        /// Gets or sets the absolute speed of the text if the speed is absolute.
        /// </summary>
        public double AbsoluteSpeed
        {
            get { return (double)GetValue(AbsoluteSpeedProperty); }
            set { SetValue(AbsoluteSpeedProperty, value); }
        }

        /// <summary>
        /// The frequency of a loop if the text speed is not absolute.
        /// </summary>
        public TimeSpan AnimationDuration
        {
            get { return (TimeSpan)GetValue(AnimationDurationProperty); }
            set { SetValue(AnimationDurationProperty, value); }
        }

        
        //public double AnimationSpeedRatio
        //{
        //    get { return (double)GetValue(AnimationSpeedRatioProperty); }
        //    set { SetValue(AnimationSpeedRatioProperty, value); }
        //}

        /// <summary>
        /// Whether or not the Marquee is a ticker or quick scroll.
        /// </summary>
        public bool IsTicker
        {
            get { return (bool)GetValue(IsTickerProperty); }
            set { SetValue(IsTickerProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether or not the Marquee scroll should repeat.
        /// </summary>
        public bool IsRepeating
        {
            get { return (bool)GetValue(IsRepeatingProperty); }
            set { SetValue(IsRepeatingProperty, value); }
        }

        /// <inheritdoc/>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            StopMarqueeAnimation(useTransitions: false);

            this.SizeChanged += MarqueeTextControl_SizeChanged;   // NOTE: This event will start animation, if needed
        }

        private void MarqueeTextControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var a = DisplayInformation.GetForCurrentView();
            var b = this.FontSize;

            StopMarqueeAnimation();
            StartMarqueeAnimationIfNeeded();
        }

        private void StopMarqueeAnimation(bool useTransitions = true)
        {
            VisualStateManager.GoToState(this, VISUALSTATE_STOPPED, useTransitions);
        }

        private void StartMarqueeAnimationIfNeeded(bool useTransitions = true)
        {
            var canvas = (Canvas)GetTemplateChild(CANVAS_NAME);
            if (canvas == null)
                return;

            // Change clip rectangle for new canvas size
            var rectanglegeometryClipCanvas = (RectangleGeometry)GetTemplateChild(RECT_GEOMETRY_CANVAS_NAME);
            if (rectanglegeometryClipCanvas != null)
                rectanglegeometryClipCanvas.Rect = new Rect(0.0d, 0.0d, canvas.ActualWidth, canvas.ActualHeight);

            if (this.IsStopped)
            {
                StopMarqueeAnimation(useTransitions);
                return;
            }

            // Add an animation handler
            var stackpanel = (StackPanel)GetTemplateChild(STACKPANEL_NAME);
            var textblock = (TextBlock)GetTemplateChild(TEXTBLOCK_NAME);
            var textblock2 = (TextBlock)GetTemplateChild(TEXTBLOCK2_NAME);
            if (textblock != null)
            {
                // Animation is only needed if 'textblock' is larger than canvas
                if (textblock.ActualWidth > canvas.ActualWidth)
                {
                    TimeSpan duration = this.AnimationDuration;
                    if (this.IsAbsoluteSpeed)
                    {
                        duration = TimeSpan.FromSeconds(textblock.ActualWidth / this.AbsoluteSpeed);
                    }

                    var visualstateGroups = VisualStateManager.GetVisualStateGroups(canvas).First();
                    var visualstateMarquee = visualstateGroups.States.Single(l => l.Name == VISUALSTATE_MARQUEE);
                    if (_storyboardMarquee != null)
                        _storyboardMarquee.Completed -= StoryboardMarquee_Completed;

                    _storyboardMarquee = new Storyboard()
                    {
                        //AutoReverse = false,
                        Duration = duration,
                        RepeatBehavior = IsRepeating ? RepeatBehavior.Forever : new RepeatBehavior(1),
                        //SpeedRatio = this.AnimationSpeedRatio
                    };

                    _storyboardMarquee.Completed += StoryboardMarquee_Completed;

                    if (IsTicker)
                    {
                        // Redundent text not neccesary if ticker.
                        textblock2.Visibility = Visibility.Collapsed;

                        var animationMarquee = new DoubleAnimationUsingKeyFrames
                        {
                            //AutoReverse = _storyboardMarquee.AutoReverse,
                            Duration = _storyboardMarquee.Duration,
                            RepeatBehavior = _storyboardMarquee.RepeatBehavior,
                            //SpeedRatio = _storyboardMarquee.SpeedRatio,
                        };
                        var frame1 = new DiscreteDoubleKeyFrame
                        {
                            KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0)),
                            Value = canvas.ActualWidth
                        };
                        var frame2 = new EasingDoubleKeyFrame
                        {
                            KeyTime = KeyTime.FromTimeSpan(_storyboardMarquee.Duration.TimeSpan),
                            Value = -textblock.ActualWidth,
                            EasingFunction = this.EasingFunction
                        };

                        animationMarquee.KeyFrames.Add(frame1);
                        animationMarquee.KeyFrames.Add(frame2);

                        _storyboardMarquee.Children.Add(animationMarquee);

                        Storyboard.SetTarget(animationMarquee, textblock.RenderTransform);
                        Storyboard.SetTargetProperty(animationMarquee, "(TranslateTransform.X)");
                    }
                    else
                    {
                        var animationMarquee = new DoubleAnimation
                        {
                            //AutoReverse = _storyboardMarquee.AutoReverse,
                            Duration = _storyboardMarquee.Duration,
                            RepeatBehavior = _storyboardMarquee.RepeatBehavior,
                            //SpeedRatio = _storyboardMarquee.SpeedRatio,
                            From = 0.0d,
                            To = -textblock.ActualWidth,
                            EasingFunction = this.EasingFunction
                        };

                        _storyboardMarquee.Children.Add(animationMarquee);

                        Storyboard.SetTarget(animationMarquee, stackpanel.RenderTransform);
                        Storyboard.SetTargetProperty(animationMarquee, "(TranslateTransform.X)");
                    }

                    visualstateMarquee.Storyboard = _storyboardMarquee;

                    VisualStateManager.GoToState(this, VISUALSTATE_MARQUEE, useTransitions);
                }
                else
                {
                    textblock2.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void StoryboardMarquee_Completed(object sender, object e)
        {
            IsStopped = true;
            if (_storyboardMarquee != null)
                _storyboardMarquee.Completed -= StoryboardMarquee_Completed;
        }
    }
}
