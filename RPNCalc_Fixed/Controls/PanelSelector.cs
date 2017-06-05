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
using System.ComponentModel;
using D = System.Diagnostics.Debug;

namespace RPNCalc.Controls {
    public sealed class PanelSelector : Panel {
        public static readonly DependencyProperty SelectedProperty =
            DependencyProperty.Register("Selected", typeof(int), typeof(PanelSelector), new PropertyMetadata(-1, OnSelectedChanged));

        static void OnSelectedChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
            var cpx = obj as PanelSelector;
            var value = (int)e.NewValue;
            cpx.UpdateSelection(value);
        }

        public int Selected {
            get { return (int)GetValue(SelectedProperty); }
            set { SetValue(SelectedProperty, value); }
        }

        private int oldCount = 0;
        public int Count {
            get { return Children.Count; }
        }

        public PanelSelector() {
            LayoutUpdated += OnLayoutUpdate;
        }

        private void UpdateSelection(int value) {
            if(value < -1 || value >= Count)
                throw new ArgumentOutOfRangeException("Selected");
            for(int i = 0; i < Count; i++)
                Children[i].Visibility = (value == i) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void OnLayoutUpdate(object sender, object e) {
            if(oldCount != Count) {
                UpdateSelection(Selected);
                oldCount = Count;
            }
        }

        protected override Size MeasureOverride(Size availableSize) {
            foreach(var ch in Children)
                if(ch.Visibility == Visibility.Visible) {
                    ch.Measure(availableSize);
                    return ch.DesiredSize;
                }
            return new Size(0, 0);
        }

        protected override Size ArrangeOverride(Size finalSize) {
            foreach(var ch in Children)
                if(ch.Visibility == Visibility.Visible) {
                    ch.Arrange(new Rect(new Point(0, 0), ch.DesiredSize));
                    return ch.DesiredSize;
                }
            return new Size(0, 0);
        }
    }
}