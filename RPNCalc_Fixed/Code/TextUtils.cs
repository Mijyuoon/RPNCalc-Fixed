using System;
using System.Collections.Generic;
using Windows.UI;
using RPNCalc.Utils;
using RPNCalc.Controls;

namespace RPNCalc {

    class TextColors {
        public static Color Number   = "#CC3C9C".ToRGB();
        public static Color LibFunc  = "#00CC44".ToRGB();
        public static Color UserFunc = "#00CC44".ToRGB();
        public static Color Register = "#2C8CCC".ToRGB();
        public static Color Constant = "#CC9628".ToRGB();
        public static Color Default  = Colors.White;

        public static Color ParseCmd(string cmd) {
            if(cmd == null)
                return Default;
            switch(cmd[0]) {
            case '#':
                return Number;
            case 'L':
                return LibFunc;
            case 'U':
                return UserFunc;
            case '$':
                return Register;
            case '.':
                return Constant;
            default:
                return Default;
            }
        }
    }

    /*class ActionTextMap {
        static Dictionary<string, string> dict = new Dictionary<string, string>();
        static HashSet<ICalcCommand> objects = new HashSet<ICalcCommand>();

        public static void Register(ICalcCommand obj) {
            objects.Add(obj);
        }

        public static void Populate() {
            foreach(var ob in objects) {
                if(!ob.Action.StartsWith("f:"))
                    continue;
                var action = ob.Action.Substring(2);
                if(char.IsLower(action[0]))
                    action = $"L{action}";
                dict[action] = ob.Text;
            }
            objects.Clear();
        }

        public static string Get(string action) {
            string text;
            if(dict.TryGetValue(action, out text))
                return text;
            switch(action[0]) {
            case 'U':
                return action.Substring(1);
            case '#':
                return double.Parse(action).ToNumeric();
            default:
                return action;
            }
        }
    }*/
}
