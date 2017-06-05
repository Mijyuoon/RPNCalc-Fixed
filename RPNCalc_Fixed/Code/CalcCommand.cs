using System;
using System.IO;
using Windows.UI;
using Windows.UI.Xaml.Media;
using RPNCalc;

namespace RPNCalc.Controls {
    public interface ICalcCommand {
        string Action { get; }
        string Text { get; }
        Brush Color { get; }
    }
    
    public struct CalcCommand : ICalcCommand {
        public string Action { get; }
        public string Text { get; }
        public Brush Color { get; }

        public CalcCommand(string action, string text) {
            var nCol = new SolidColorBrush(TextColors.ParseCmd(action));
            Action = action; Text = text; Color = nCol;
        }
        public void Serialize(BinaryWriter writer) {
            writer.Write(Action);
            writer.Write(Text);
        }
        public static CalcCommand Deserialize(BinaryReader reader) {
            var action = reader.ReadString();
            var text = reader.ReadString();
            return new CalcCommand(action, text);
            //return new CalcCommand(action, ActionTextMap.Get(action));
        }
    }

    public class CalcCommandEventArgs : EventArgs {
        public string Action { get; }
        public string Text { get; }
        public Brush Color { get; }

        public CalcCommandEventArgs(ICalcCommand obj) {
            Action = obj.Action; Text = obj.Text; Color = obj.Color;
        }
    }

    public static class CalcCommandGlobalEvent {
        public static event EventHandler<CalcCommandEventArgs> OnAction;

        public static void Emit(ICalcCommand obj) {
            if(OnAction != null && !string.IsNullOrEmpty(obj.Action))
                OnAction(obj, new CalcCommandEventArgs(obj));
        }
    }
}