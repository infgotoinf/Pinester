using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Pinester
{
    public class MasonryPanel : Panel
    {
        public static readonly DependencyProperty ColumnWidthProperty =
            DependencyProperty.Register("ColumnWidth", typeof(double), typeof(MasonryPanel),
                new FrameworkPropertyMetadata(200.0,
                    FrameworkPropertyMetadataOptions.AffectsArrange |
                    FrameworkPropertyMetadataOptions.AffectsMeasure));

        public double ColumnWidth
        {
            get => (double)GetValue(ColumnWidthProperty);
            set => SetValue(ColumnWidthProperty, value);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (InternalChildren.Count == 0)
                return new Size(0, 0);

            int numColumns = CalculateColumnCount(availableSize.Width);
            double[] columnHeights = new double[numColumns];

            foreach (UIElement child in InternalChildren)
            {
                child.Measure(new Size(ColumnWidth, double.PositiveInfinity));
                int columnIndex = GetShortestColumnIndex(columnHeights);
                columnHeights[columnIndex] += child.DesiredSize.Height;
            }

            return new Size(availableSize.Width, columnHeights.Max());
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (InternalChildren.Count == 0)
                return finalSize;

            int numColumns = CalculateColumnCount(finalSize.Width);
            double[] columnHeights = new double[numColumns];

            // Calculate centering offset
            double totalWidth = numColumns * ColumnWidth;
            double horizontalOffset = (finalSize.Width - totalWidth) / 2;

            foreach (UIElement child in InternalChildren)
            {
                int columnIndex = GetShortestColumnIndex(columnHeights);
                double x = horizontalOffset + (columnIndex * ColumnWidth);
                double y = columnHeights[columnIndex];

                child.Arrange(new Rect(x, y, ColumnWidth, child.DesiredSize.Height));
                columnHeights[columnIndex] += child.DesiredSize.Height;
            }

            return finalSize;
        }

        private int CalculateColumnCount(double availableWidth)
        {
            int maxColumns = Math.Max(1, (int)(availableWidth / ColumnWidth));
            return Math.Min(maxColumns, InternalChildren.Count);
        }

        private int GetShortestColumnIndex(double[] columnHeights)
        {
            int index = 0;
            double minHeight = columnHeights[0];

            for (int i = 1; i < columnHeights.Length; i++)
            {
                if (columnHeights[i] < minHeight)
                {
                    minHeight = columnHeights[i];
                    index = i;
                }
            }
            return index;
        }
    }
}