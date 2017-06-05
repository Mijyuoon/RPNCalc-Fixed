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
using RPNCalc.Dialogs;
using RPNCalc.Controls;

namespace RPNCalc.Keyboards {
    public sealed partial class Registers : UserControl {
        public Registers() {
            InitializeComponent();
        }

        private async void OnInsertRegister(object sender, RoutedEventArgs e) {
            var dialog = new InsertRegister();
            await dialog.ShowAsync();

            var reg = dialog.RegisterName;
            if(reg != null) {
                var struc = new CalcCommand($"f:$r{reg}", reg.ToLower());
                CalcCommandGlobalEvent.Emit(struc);
            }
        }
    }
}
