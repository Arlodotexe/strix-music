/// https://stackoverflow.com/a/15615736/

using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

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

            // TODO: Remove ButtonBase and PivotItem, just use TextBlocks
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

            d.RegisterPropertyChangedCallback(prop, (s, e) =>
            {
                if (mutex)
                    return;

                mutex = true;
                d.SetValue(prop, CaseConverter.Convert((string)d.GetValue(prop), val));
                mutex = false;
            });
        }
    }
}
