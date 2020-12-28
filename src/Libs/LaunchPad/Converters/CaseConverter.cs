/// https://stackoverflow.com/a/29841405

using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace LaunchPad
{
    public class CaseConverter : IValueConverter
    {
        public CharacterCasing Case { get; set; }

        public CaseConverter()
        {
            Case = CharacterCasing.Upper;
        }

        public static string Convert(string? value, CharacterCasing characterCasing)
        {
            if (value != null)
            {
                switch (characterCasing)
                {
                    case CharacterCasing.Lower:
                        return value.ToLower();
                    case CharacterCasing.Normal:
                        return value;
                    case CharacterCasing.Upper:
                        return value.ToUpper();
                    default:
                        return value;
                }
            }
            return string.Empty;
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var str = value as string;
            return Convert(str, Case);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
