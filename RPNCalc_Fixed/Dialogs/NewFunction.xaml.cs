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
using RPNCalc;

namespace RPNCalc.Dialogs {
    public sealed partial class NewFunction : ContentDialog {
        public NewFunction() {
            InitializeComponent();
        }

        public NewFunction(CalcManager.Function func) : this() {
            if(func.Name != null) {
                tName.Text = func.Name;
                tDesc.Text = func.Descr ?? "";
                cbParam.SelectedIndex = func.ParamNum;
                tName.IsEnabled = false;
            }
        }

        public string FunctionName { get; private set; } = null;
        public string FunctionDescr { get; private set; } = null;
        public int ParameterCount { get; private set; } = 0;

        private void OnOK(ContentDialog sender, ContentDialogButtonClickEventArgs args) {
            var fname = tName.Text.ToLower().Trim();
            var regex = new Regex(@"^[a-z_][a-z0-9_]*$");
            if(regex.Match(fname).Success) {
                FunctionName = fname;
                FunctionDescr = tDesc.Text.Trim();
                ParameterCount = cbParam.SelectedIndex;
            }
        }
    }
}
