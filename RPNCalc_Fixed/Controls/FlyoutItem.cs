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
    public sealed class FlyoutItem : MenuFlyoutItem, ICalcCommand {
        public FlyoutItem() : base() {
            Click += OnClicked;
        }

        public string Action { get; set; } = null;
        public Brush Color { get; set; } = null;

        private void OnClicked(object sender, RoutedEventArgs e) {
            CalcCommandGlobalEvent.Emit(this);
        }
    }
}