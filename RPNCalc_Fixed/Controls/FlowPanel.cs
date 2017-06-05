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
using System.Diagnostics;

namespace RPNCalc.Controls {
    public sealed class FlowPanel : Panel {
        public static readonly DependencyProperty SpacingProperty =
            DependencyProperty.Register("Spacing", typeof(double), typeof(FlowPanel), new PropertyMetadata(0.0));
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(FlowPanel), new PropertyMetadata(Orientation.Horizontal));

        public double Spacing {
            get { return (double)GetValue(SpacingProperty); }
            set { SetValue(SpacingProperty, value); }
        }
        public Orientation Orientation {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public FlowPanel() {
            // Nothing
        }

        protected override Size MeasureOverride(Size availableSize) {
            double newWidth, newHeight;

            if(Orientation == Orientation.Horizontal) {
                newWidth = availableSize.Width;
                newHeight = 0.0;

                double lineWidth = 0.0;
                double maxSubHeight = 0.0;

                foreach(var child in Children) {
                    child.Measure(availableSize);

                    var tmpWidth = lineWidth + child.DesiredSize.Width + Spacing;

                    if(tmpWidth > availableSize.Width) {
                        newHeight += Spacing + maxSubHeight;
                        lineWidth = child.DesiredSize.Width;
                        maxSubHeight = child.DesiredSize.Height;
                    } else {
                        lineWidth = tmpWidth;
                        maxSubHeight = Math.Max(maxSubHeight, child.DesiredSize.Height);
                    }
                }

                newHeight += maxSubHeight;
            } else {
                throw new NotSupportedException("Really?");
            }
            
            return new Size(Math.Max(0, newWidth), Math.Max(0, newHeight));
        }

        protected override Size ArrangeOverride(Size finalSize) {
            var coord = new Point();

            if(Orientation == Orientation.Horizontal) {
                double maxSubHeight = 0.0;

                foreach(var child in Children) {
                    double newX = coord.X + child.DesiredSize.Width;

                    if(newX > finalSize.Width) {
                        newX -= coord.X;
                        coord.X = 0.0;
                        coord.Y += maxSubHeight + Spacing;
                        maxSubHeight = 0.0;
                    }

                    child.Arrange(new Rect(coord, child.DesiredSize));

                    coord.X = newX + Spacing;
                    maxSubHeight = Math.Max(maxSubHeight, child.DesiredSize.Height);
                }
            } else {
                throw new NotSupportedException("Really?");
            }
            
            return finalSize;
        }
    }
}