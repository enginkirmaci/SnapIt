using SnapIt.Library.Controls;
using SnapIt.Library.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace SnapIt.Library.Extensions
{
    public static class SnapBorderExtensions
    {
        public static List<SnapBorder> GetCollisions(this SnapBorder snapBorder, Rect rect, SplitDirection splitDirection)
        {
            var borders = snapBorder.Parent.FindChildren<SnapBorder>();

            var result = borders
                .Where(b => b.SplitDirection == splitDirection && rect.IntersectsWith(b.GetRect()))
                .ToList();

            result.Remove(snapBorder);

            return result;
        }

        public static List<SnapBorder> GetCollisions(this SnapBorder snapBorder, Rect rect)
        {
            var borders = snapBorder.Parent.FindChildren<SnapBorder>();

            var result = borders
                .Where(b => rect.IntersectsWith(b.GetRect()))
                .ToList();

            result.Remove(snapBorder);

            return result;
        }

        public static Rect GetRect(this SnapBorder snapBorder)
        {
            return snapBorder.SplitDirection == SplitDirection.Vertical ?
                           new Rect(
                               snapBorder.Margin.Left,
                               snapBorder.Margin.Top,
                               SnapBorder.THICKNESS,
                               snapBorder.ActualHeight) :
                       new Rect(
                               snapBorder.Margin.Left,
                               snapBorder.Margin.Top,
                               snapBorder.ActualWidth,
                               SnapBorder.THICKNESS);
        }
    }
}