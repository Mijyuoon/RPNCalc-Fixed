using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace RPNCalc.Controls {
    public sealed class UniformGrid : Panel {
        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register("Columns", typeof(int), typeof(UniformGrid), new PropertyMetadata(1, OnColumnsChanged));
        public static readonly DependencyProperty RowsProperty =
            DependencyProperty.Register("Rows", typeof(int), typeof(UniformGrid), new PropertyMetadata(1, OnRowsChanged));
        /*public static readonly DependencyProperty ItemWidthProperty =
            DependencyProperty.Register("ItemWidth", typeof(double), typeof(UniformGrid), new PropertyMetadata(0.0, null));
        public static readonly DependencyProperty ItemHeightProperty =
            DependencyProperty.Register("ItemHeight", typeof(double), typeof(UniformGrid), new PropertyMetadata(0.0, null));*/
        public static readonly DependencyProperty AspectRatioProperty =
            DependencyProperty.Register("AspectRatio", typeof(double), typeof(UniformGrid), new PropertyMetadata(1.0));
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(UniformGrid), new PropertyMetadata(Orientation.Vertical));
        public static readonly DependencyProperty CellSpanProperty =
            DependencyProperty.RegisterAttached("CellSpan", typeof(int), typeof(UniformGrid), new PropertyMetadata(1));

        public static int GetCellSpan(UIElement element) {
            return (int)element.GetValue(CellSpanProperty);
        }
        public static void SetCellSpan(UIElement element, int value) {
            element.SetValue(CellSpanProperty, value);
        }

        static void OnColumnsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
            int value = (int)e.NewValue;
            (obj as UniformGrid).Columns = value > 1 ? value : 1;
        }
        static void OnRowsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
            int value = (int)e.NewValue;
            (obj as UniformGrid).Rows = value > 1 ? value : 1;
        }

        public int Columns {
            get { return (int)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }
        public int Rows {
            get { return (int)GetValue(RowsProperty); }
            set { SetValue(RowsProperty, value); }
        }
        /*public double ItemWidth {
            get { return (double)GetValue(ItemWidthProperty); }
            set { SetValue(ItemWidthProperty, value); }
        }
        public double ItemHeight {
            get { return (double)GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }*/
        public double AspectRatio {
            get { return (double)GetValue(AspectRatioProperty); }
            set { SetValue(AspectRatioProperty, value); }
        }
        public Orientation Orientation {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public UniformGrid() {
            // Nothing
        }

        protected override Size MeasureOverride(Size availableSize) {
            double finalWidth, finalHeight;
            int vcount = Children.Count(x => x.Visibility == Visibility.Visible);
            if(Orientation == Orientation.Horizontal) {
                finalWidth = availableSize.Width;
                //var itemWidth = Math.Floor(availableSize.Width / Columns);
                var itemWidth = availableSize.Width / Columns;
                var actualRows = Math.Ceiling((double)vcount / Columns);
                //var actualHeight = Math.Floor(availableSize.Height / actualRows);
                var actualHeight = availableSize.Height / actualRows;
                //var itemHeight = Math.Min(actualHeight, ItemHeight > 0 ? ItemHeight : itemWidth);
                var itemHeight = Math.Min(actualHeight, itemWidth / AspectRatio);
                finalHeight = itemHeight * actualRows;
                
                foreach(var child in Children)
                    child.Measure(new Size(itemWidth, itemHeight));
            } else {
                finalHeight = availableSize.Height;
                //var itemHeight = Math.Floor(availableSize.Height / Rows);
                var itemHeight = availableSize.Height / Rows;
                var actualColumns = Math.Ceiling((double)vcount / Rows);
                //var actualWidth = Math.Floor(availableSize.Width / actualColumns);
                var actualWidth = availableSize.Width / actualColumns;
                //var itemWidth = Math.Min(actualWidth, ItemWidth > 0 ? ItemWidth : itemHeight);
                var itemWidth = Math.Min(actualWidth, itemHeight / AspectRatio);
                finalWidth = itemWidth * actualColumns;
                foreach(var child in Children)
                    child.Measure(new Size(itemWidth, itemHeight));
            }
            return new Size(finalWidth, finalHeight);
        }

        protected override Size ArrangeOverride(Size finalSize) {
            int vcount = Children.Count(x => x.Visibility == Visibility.Visible);
            if(Orientation == Orientation.Horizontal) {
                var actualRows = Math.Ceiling((double)vcount / Columns);
                //var cellWidth = Math.Floor(finalSize.Width / Columns);
                //var cellHeight = Math.Floor(finalSize.Height / actualRows);
                var cellWidth = finalSize.Width / Columns;
                var cellHeight = finalSize.Height / actualRows;
                Size cellSize = new Size(cellWidth, cellHeight);
                int row = 0, col = 0;
                foreach(UIElement child in Children) {
                    if(child.Visibility != Visibility.Visible) continue;
                    child.Arrange(new Rect(new Point(cellSize.Width * col, cellSize.Height * row), cellSize));
                    var element = child as FrameworkElement;
                    if(element != null) {
                        element.Height = cellSize.Height;
                        element.Width = cellSize.Width;
                    }

                    if(++col == Columns) {
                        row++; col = 0;
                    }
                }
            } else {
                var actualColumns = Math.Ceiling((double)vcount / Rows);
                //var cellWidth = Math.Floor(finalSize.Width / actualColumns);
                //var cellHeight = Math.Floor(finalSize.Height / Rows);
                var cellWidth = finalSize.Width / actualColumns;
                var cellHeight = finalSize.Height / Rows;
                Size cellSize = new Size(cellWidth, cellHeight);
                int row = 0, col = 0;
                foreach(UIElement child in Children) {
                    if(child.Visibility != Visibility.Visible) continue;
                    child.Arrange(new Rect(new Point(cellSize.Width * col, cellSize.Height * row), cellSize));
                    var element = child as FrameworkElement;
                    if(element != null) {
                        element.Height = cellSize.Height;
                        element.Width = cellSize.Width;
                    }

                    if(++row == Rows) {
                        col++; row = 0;
                    }
                }
            }
            return finalSize;
        }
    }
}
