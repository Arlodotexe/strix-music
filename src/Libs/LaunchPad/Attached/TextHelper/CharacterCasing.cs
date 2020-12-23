/// https://stackoverflow.com/a/15615736/

using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;

namespace LaunchPad.Attached
{
    public partial class TextHelpers : DependencyObject
    {
        public static CharacterCasing GetCharacterCasing(DependencyObject obj)
        {
            return (CharacterCasing)obj.GetValue(CharacterCasingProperty);
        }

        public static void SetCharacterCasing(DependencyObject obj, CharacterCasing value)
        {
            obj.SetValue(CharacterCasingProperty, value);
        }

        // Using a DependencyProperty as the backing store for Casing.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CharacterCasingProperty =
            DependencyProperty.RegisterAttached("CharacterCasing",
                typeof(TextHelpers),
                typeof(CharacterCasing),
                new PropertyMetadata(CharacterCasing.Normal, OnCharacterCasingChanged));

        private static bool mutex; // This is nasty

        private static void OnCharacterCasingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var val = (CharacterCasing)e.NewValue;
            DependencyProperty prop;
            var fe = (FrameworkElement)d;

            switch(d)
            {
                case TextBlock txt:
                    prop = TextBlock.TextProperty;
                    break;
                case ButtonBase hBtn:
                    prop = ButtonBase.ContentProperty;
                    break;
                case PivotItem pvi:
                    prop = PivotItem.HeaderProperty;
                    break;
                default:
                    throw new ArgumentException();
            }

            fe.RegisterPropertyChangedCallback(prop, (s, e) =>
            {
                if (mutex)
                    return;

                mutex = true;
                switch (d)
                {
                    case TextBlock txt:
                        txt.Text = CaseConverter.Convert(txt.Text, val);
                        break;
                    case ButtonBase btn:
                        btn.Content = CaseConverter.Convert((string)btn.Content, val);
                        break;
                    case PivotItem pvi:
                        pvi.Header = CaseConverter.Convert((string)pvi.Header, val);
                        break;
                    default:
                        throw new ArgumentException();
                }
                mutex = false;
            });
        }
    }
}
