using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using RPNCalc.Utils;

namespace RPNCalc.Controls {
    public sealed partial class ProgramBox : UserControl {
        public ProgramBox() {
            InitializeComponent();
        }

        private int selection = -1;
        public int Selection {
            get { return selection; }
            set {
                if(selection > -1)
                    SelectItem(selection, false);
                if(value > -1)
                    SelectItem(value, true);
                selection = value;
            }
        }

        public ICalcCommand this[int index] {
            get {
                if(index < 0 || index >= ItemCount) return null;
                return pBox.Children[index] as ICalcCommand;
            }
        }

        public int ItemCount {
            get { return pBox.Children.Count; }
        }

        private void SelectItem(int index, bool value) {
            if(index < 0 || index >= ItemCount) return;
            (pBox.Children[index] as ProgramBoxItem).Selected = value;
        }

        private void AdjustSelection(int value) {
            value = (value + 1).Mod(ItemCount + 1) - 1;
            var delta = (value - Selection > 0) ? 1 : -1;
            while(value > -1 && this[value].Action == null)
                value += (value + delta + 1).Mod(ItemCount + 1) - 1;
            Selection = value;
        }

        public void AddItem(ICalcCommand obj) {
            var box = new ProgramBoxItem() {
                Action = obj.Action,
                Text = obj.Text,
                Color = obj.Color,
            };
            if(Selection < 0) {
                pBox.Children.Add(box);
            } else {
                pBox.Children.Insert(Selection, box);
                AdjustSelection(Selection+1);
            }
        }

        public void AddItem(string action, string text) {
            AddItem(new CalcCommand(action, text));
        }

        public void RemoveItem() {
            if(ItemCount < 1) return;
            var idx = (Selection < 0) ? ItemCount-1 : Selection;
            Selection = -1;
            pBox.Children.RemoveAt(idx);
            if(idx < ItemCount)
                AdjustSelection(idx);
        }

        public void ClearItems() {
            Selection = -1;
            pBox.Children.Clear();
        }

        public string[] CompiledItems() {
            return pBox.Children
                .Cast<ProgramBoxItem>()
                .Where(x => x.Action != null)
                .Select(x => x.Action)
                .ToArray();
        }

        public CalcCommand[] Items() {
            return pBox.Children
                .Cast<ProgramBoxItem>()
                .Where(x => x.Action != null)
                .Select(x => new CalcCommand(x.Action, x.Text))
                .ToArray();
        }

        private void OnSelection(object sender, RoutedEventArgs e) {
            var incr = Convert.ToInt32((sender as FrameworkElement).Tag);
            AdjustSelection((incr == 0) ? -1 : Selection + incr);
        }

        private void OnFlyout(object sender, TappedRoutedEventArgs e) {
            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        }
    }
}