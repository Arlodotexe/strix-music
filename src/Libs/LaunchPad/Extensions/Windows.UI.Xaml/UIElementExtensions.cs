using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Windows.UI.Xaml
{
    /// <summary>
    /// A collection of extension methods for <see cref="UIElement"/>
    /// </summary>
    public static class UIElementExtensions
    {
        public static T FindChild<T>(this DependencyObject parent) where T : class
        {
            if (parent is T tParent)
            {
                return tParent;
            }

            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T tChild)
                {
                    return tChild;
                }
                else
                {
                    T tFromChild = child.FindChild<T>();
                    if (tFromChild != null)
                    {
                        return tFromChild;
                    }
                }
            }

            return null;
        }
    }
}
