using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace OwlCore.WinUI.Extensions.Windows.UI.Xaml
{
    /// <summary>
    /// A collection of extension methods for <see cref="UIElement"/>
    /// </summary>
    public static class UIElementExtensions
    {
        /// <summary>
        /// Finds the first child of a specified type on a given <see cref="DependencyObject"/>.
        /// </summary>
        /// <typeparam name="T">The type to look for.</typeparam>
        /// <param name="parent">The object to search through.</param>
        /// <returns>The located object, if found.</returns>
        public static T FindChild<T>(this DependencyObject parent) where T : class
        {
            if (parent is T tParent)
            {
                return tParent;
            }

            var childCount = VisualTreeHelper.GetChildrenCount(parent);

            for (var i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T tChild)
                {
                    return tChild;
                }

                var tFromChild = child.FindChild<T>();
                if (tFromChild != null)
                {
                    return tFromChild;
                }
            }

            return null!;
        }
    }
}
