using System;
using System.Collections.Generic;
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

namespace RPNCalc.Dialogs {
    public sealed partial class CalcUserQuery : ContentDialog {
        public CalcUserQuery() {
            InitializeComponent();
        }

        public double Value { get; private set; } = 0.0;

        private void OnOK(ContentDialog sender, ContentDialogButtonClickEventArgs args) {
            double result = 0.0;
            var text = tVal.Text.Trim();
            double.TryParse(text, out result);
            Value = result;
        }
    }
}
