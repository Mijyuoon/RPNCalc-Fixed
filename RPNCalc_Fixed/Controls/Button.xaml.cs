using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace RPNCalc.Controls {
    public sealed partial class Button : UserControl, ICalcCommand {
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(Button), null);
        /*public static readonly DependencyProperty AddContentProperty =
            DependencyProperty.Register("AddContent", typeof(object), typeof(CalcButton), null);*/
        /*public static readonly DependencyProperty TextStyleProperty =
            DependencyProperty.Register("TextStyle", typeof(TextStyle), typeof(Button), new PropertyMetadata(TextStyle.N, OnTextStyleChange));

        static void OnTextStyleChange(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
            var cpx = (Button)obj;
            var value = (TextStyle)e.NewValue;
            cpx.btn.FontStyle = ((value & TextStyle.I) != 0) ? FontStyle.Italic : FontStyle.Normal;
            cpx.btn.FontWeight = ((value & TextStyle.B) != 0) ? FontWeights.Bold : FontWeights.Normal;
        }*/

        public string Text {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        /*public object AddContent {
            get { return GetValue(AddContentProperty); }
            set { SetValue(AddContentProperty, value); }
        }*/
        private TextStyle _style = TextStyle.N;
        public TextStyle TextStyle {
            /*get { return (TextStyle)GetValue(TextStyleProperty); }
            set { SetValue(TextStyleProperty, value); }*/
            get { return _style; }
            set {
                _style = value;
                btn.FontStyle = ((value & TextStyle.I) != 0) ? FontStyle.Italic : FontStyle.Normal;
                btn.FontWeight = ((value & TextStyle.B) != 0) ? FontWeights.Bold : FontWeights.Normal;
            }
        }
        public string Action { get; set; } = null;
        public Brush Color { get; set; } = null;
        public bool HasFlyout {
            get { return (FlyoutBase.GetAttachedFlyout(this) != null); }
        }

        public Button() {
            InitializeComponent();
            //ActionTextMap.Register(this);
        }

        private void OnFlyout(object sender, HoldingRoutedEventArgs e) {
            var flt = FlyoutBase.GetAttachedFlyout(this);
            if(flt != null) {
                flt.Placement = FlyoutPlacementMode.Bottom;
                flt.ShowAt(this);
            }
        }

        public event RoutedEventHandler Click;
        private void OnClicked(object sender, RoutedEventArgs e) {
            if(Click != null)
                Click(this, new RoutedEventArgs());
            switch(Action) {
            case "*":
                OnFlyout(this, null);
                break;
            default:
                CalcCommandGlobalEvent.Emit(this);
                break;
            }
        }
    }
}
