using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace RPNCalc.Utils {
    internal sealed class BooleanToVisibilityConverter : IValueConverter {
        public bool IsReversed { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language) {
            var val = System.Convert.ToBoolean(value);
            if(IsReversed) val = !val;
            return val ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            throw new NotImplementedException();
        }
    }

    internal sealed class MathMultiplyConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, string language) {
            var mul = System.Convert.ToDouble(parameter);
            var src = System.Convert.ToDouble(value);
            return src * mul;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            throw new NotImplementedException();
        }
    }

    public class PropertyNotifier : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        public void SetProperty<T>(ref T prop, T value, [CallerMemberName] string name = null) {
            if(!EqualityComparer<T>.Default.Equals(prop, value)) {
                prop = value;
                if(name != null && PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }

    internal static class ExtensionUtils {
        public static string FixNumeric(this string input) {
            switch(input) {
            case "Infinity":
                return "+∞";
            case "-Infinity":
                return "−∞";
            case "NaN":
                return "∅";
            default:
                return input.Replace('-', '−');
            }
        }

        public static string ToNumeric(this double input) {
            return input.ToString("0.#################", CultureInfo.InvariantCulture).FixNumeric();
        }

        public static Tuple<string,string> ToNumericExp(this double input) {
            var str = input.ToString("g", CultureInfo.InvariantCulture).FixNumeric().Split('e');
            return new Tuple<string, string>(str[0], str.Length > 1 ? str[1] : "");
        }

        public static Color ToRGB(this string input) {
            if(input.Length != 7 || !input.StartsWith("#"))
                throw new ArgumentException("invalid color format");
            var r = byte.Parse(input.Substring(1, 2), NumberStyles.HexNumber);
            var g = byte.Parse(input.Substring(3, 2), NumberStyles.HexNumber);
            var b = byte.Parse(input.Substring(5, 2), NumberStyles.HexNumber);
            return Color.FromArgb(255, r, g, b);
        }

        public static int Mod(this int value, int mod) {
            return ((value % mod) + mod) % mod;
        }
    }
}

namespace RPNCalc.Controls {
    public enum TextStyle {
        N  = 0,
        B  = 1,
        I  = 2,
        BI = 3,
    }
}
