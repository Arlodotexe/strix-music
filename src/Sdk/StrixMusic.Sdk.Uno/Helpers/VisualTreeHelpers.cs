using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace StrixMusic.Sdk.Uno.Helpers
{
    public static class VisualTreeHelpers
    {
        /// <summary>
        /// Finds a child item through any levels in the VisualTree.
        /// </summary>
        /// <typeparam name="T">The type of item being searched for.</typeparam>
        /// <param name="depObj">The parents item to be searched.</param>
        /// <returns>All children of <typeparamref name="T"/> child to <paramref name="depObj"/>.</returns>
        /// <remarks>
        /// https://stackoverflow.com/questions/34396002/how-to-get-element-in-code-behind-from-datatemplate
        /// </remarks>
        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child!))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        /// <summary>
        /// Finds a child item through any levels in the VisualTree with name a certain name.
        /// </summary>
        /// <typeparam name="T">The type of the item being searched for.</typeparam>
        /// <param name="depObj">The parent item to be searched</param>
        /// <param name="childName">The name of the object being searched for.</param>
        /// <returns>The first child found of type <typeparamref name="T"/> with the name <paramref name="childName"/>.</returns>
        /// <remarks>
        /// https://stackoverflow.com/questions/34396002/how-to-get-element-in-code-behind-from-datatemplate
        /// </remarks>
        public static T GetDataTemplateChild<T>(DependencyObject depObj, string? childName = null) where T : FrameworkElement
        {
            foreach (var item in FindVisualChildren<T>(depObj))
            {
                if (childName == null || item.Name == childName)
                {
                    return item;
                }
            }

            return null!;
        }

    }
}
