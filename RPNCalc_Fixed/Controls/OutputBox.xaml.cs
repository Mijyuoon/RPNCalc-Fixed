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
using RPNCalc.Utils;

namespace RPNCalc.Controls {
    public sealed partial class OutputBox : UserControl {
        private bool mode = false;
        private string input = "";
        private double? result = null;
        private string message = null;
        
        public double? OutResult {
            get { return result; }
            set {
                result = value;
                message = null;
                if(!IsInputMode)
                    UpdateTexts();
            }
        }

        public string Message {
            get { return message; }
            set {
                message = value;
                result = null;
                if(!IsInputMode)
                    UpdateTexts();
            }
        }

        public bool IsInputMode {
            get { return mode; }
            set {
                bSgn.IsEnabled = mode = value;
                //UpdateTexts();
            }
        }

        public OutputBox() {
            InitializeComponent();
            IsInputMode = false;
            UpdateTexts();
        }

        public void SendInput(char ch) {
            if(!IsInputMode) {
                input = "+";
                IsInputMode = true;
            }
            switch(ch) {
            case '+': case '-':
                var s = (input[0] == '-') ? '+' : '-';
                input = s + input.Substring(1);
                break;
            case '.': case 'p':
                if(input.IndexOf('.') > -1)
                    break;
                if(input.Length < 2)
                    input += '0';
                input += '.';
                break;
            case '1': case '2': case '3':
            case '4': case '5': case '6':
            case '7': case '8': case '9':
            case '0':
                input += ch;
                break;
            case '<':
                input = input.Substring(0, input.Length-1);
                if(input.Length < 1)
                    IsInputMode = false;
                break;
            case '*':
                IsInputMode = false;
                break;
            }
            if(input.Length > 18)
                input = input.Substring(0, 18);
            UpdateTexts();
        }

        public double FinishInput() {
            if(!mode)
                return double.NaN;
            double res = double.NaN;
            double.TryParse(input, out res);
            IsInputMode = false;
            UpdateTexts();
            return res;
        }

        private void UpdateTexts() {
            var nCol = IsInputMode ? TextColors.Number : TextColors.Default;
            lMan.Foreground = new SolidColorBrush(nCol);
            if(IsInputMode) {
                lMan.Text = input.FixNumeric();
                lExp.Text = "";
            } else if(result.HasValue) {
                var vals = result.Value.ToNumericExp();
                lMan.Text = vals.Item1;
                lExp.Text = vals.Item2;
            } else {
                lMan.Text = message ?? "";
                lExp.Text = "";
            }
        }

        private void OnChangeSign(object sender, RoutedEventArgs e) {
            if(IsInputMode) SendInput('+');
        }
    }
}
