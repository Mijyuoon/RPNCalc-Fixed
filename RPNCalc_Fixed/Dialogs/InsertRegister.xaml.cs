using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
    public sealed partial class InsertRegister : ContentDialog {
        public InsertRegister() {
            InitializeComponent();
        }

        public string RegisterName { get; private set; } = null;

        private void OnOK(ContentDialog sender, ContentDialogButtonClickEventArgs args) {
            var text = tReg.Text.ToUpper().Trim();
            var regex = new Regex(@"^[A-Z_][A-Z0-9_]*$");
            if(regex.Match(text).Success)
                RegisterName = text;
        }
    }
}
