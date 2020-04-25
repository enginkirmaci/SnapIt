using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace SnapIt.Library.Extensions
{
    public static class DependencyObjectExtensions
    {
        public static IEnumerable<T> FindChildren<T>(this DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    var child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        public static T FindParent<T>(this DependencyObject child) where T : DependencyObject
        {
            var parent = VisualTreeHelper.GetParent(child);

            if (parent == null)
            {
                return null;
            }

            if (parent != null && parent is T)
            {
                return (T)parent;
            }
            else
            {
                return FindParent<T>(parent);
            }
        }
    }
}