using System;
using System.Collections.Generic;
using System.Windows;

namespace SnapIt.Library.Entities
{
    public class LayoutArea
    {
        public string Name { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public int Column { get; set; }
        public int Row { get; set; }
        public int ColumnSpan { get; set; }
        public int RowSpan { get; set; }
        public List<LayoutArea> Areas { get; set; }

        public bool IsAt(int x, int y)
        {
            var columnIndex = Column * ColumnSpan;
            if (columnIndex == 0 && ColumnSpan != 1)
            {
                columnIndex = ColumnSpan - 1;
            }

            var rowIndex = Row * RowSpan;
            if (rowIndex == 0 && RowSpan != 1)
            {
                rowIndex = RowSpan - 1;
            }

            return Column <= x && x <= columnIndex &&
                   Row <= y && y <= rowIndex;
        }

        public bool HasCollision(LayoutArea layoutArea)
        {
            Rect rect1 = new Rect(Column, Row, ColumnSpan, RowSpan);
            Rect rect2 = new Rect(layoutArea.Column + 0.1, layoutArea.Row + 0.1, layoutArea.ColumnSpan - 0.1, layoutArea.RowSpan - 0.1);

            return rect1.IntersectsWith(rect2);
        }

        public LayoutArea Copy()
        {
            return new LayoutArea
            {
                Name = Name + " >" + new Random().Next(0, 100),
                Width = Width,
                Height = Height,
                Column = Column,
                Row = Row,
                ColumnSpan = ColumnSpan,
                RowSpan = RowSpan,
                Areas = Areas
            };
        }
    }
}