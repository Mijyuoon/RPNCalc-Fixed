using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
using RPNCalc.Controls;
using RPNCalc.Dialogs;
using RPNCalc.Utils;
using RPNCalcLib;

namespace RPNCalc {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page {
        public MainPage() {
            InitializeComponent();
            kbds.Selected = 0;
            EditFunction = null;
            NavigationCacheMode = NavigationCacheMode.Required;
            CalcCommandGlobalEvent.OnAction += OnAction;
        }

        private CalcManager.Function editFunc;
        public CalcManager.Function EditFunction {
            get { return editFunc; }
            set {
                editFunc = value;
                if(value != null) {
                    program.ClearItems();
                    output.OutResult = null;
                    program.AddItem(null, $"fn({value.Name})");
                    foreach(var c in value.Code)
                        program.AddItem(c);
                }
            }
        }

        public void InsertFunction(CalcManager.Function func) {
            if(output.IsInputMode)
                HandleSys("exe");
            program.AddItem($"U{func.Name}", func.Name);
        }

        private void OnNavigateTo(object sender, RoutedEventArgs e) {
            var ob = sender as FrameworkElement;
            Frame.Navigate(Type.GetType((string)ob.Tag), this);
        }

        private void OnSwitchKeyboard(object sender, RoutedEventArgs e) {
            var cpx = sender as FrameworkElement;
            if(cpx.Tag == null) {
                FlyoutBase.ShowAttachedFlyout(cpx);
            } else {
                kbds.Selected = Convert.ToInt32(cpx.Tag);
            }
        }

        private void OnAction(object sender, CalcCommandEventArgs e) {
            var data = e.Action.Substring(2);
            switch(e.Action[0]) {
            case 'd':
                output.SendInput(data[0]);
                break;
            case 's':
                HandleSys(data);
                break;
            case 'f':
                if(output.IsInputMode)
                    HandleSys("exe");
                if(char.IsLower(data[0]))
                    data = $"L{data}";
                program.AddItem(data, e.Text);
                break;
            }
        }

        private async void HandleSys(string cmd) {
            switch(cmd) {
            case "clr":
                if(output.IsInputMode) {
                    output.SendInput('*');
                } else {
                    EditFunction = null;
                    output.OutResult = null;
                    program.ClearItems();
                }
                break;
            case "del":
                if(output.IsInputMode) {
                    output.SendInput('<');
                } else {
                    program.RemoveItem();
                }
                break;
            case "exe":
                if(output.IsInputMode) {
                    var num = output.FinishInput();
                    program.AddItem($"#{num}", num.ToNumeric());
                } else if(editFunc != null) {
                    var isNew = (editFunc.Name == null);
                    var dialog = new NewFunction(editFunc);
                    await dialog.ShowAsync();
                    if(dialog.FunctionName != null) {
                        var app = (Application.Current as App);
                        if(isNew) {
                            editFunc.Name = dialog.FunctionName;
                            app.CalcManager.Functions.Add(editFunc);
                        }
                        editFunc.Descr = dialog.FunctionDescr;
                        editFunc.ParamNum = dialog.ParameterCount;
                        editFunc.Code = program.Items();
                        var fn = editFunc.Compile(app.Calculator);
                        app.Calculator.SetUserFunc(editFunc.Name, fn);
                        program.ClearItems();
                        EditFunction = null;
                    }
                } else {
                    try {
                        var calc = (Application.Current as App).Calculator;
                        var fn = calc.Compile(program.CompiledItems(), 0);
                        output.OutResult = calc.Execute(fn);
                    } catch(Exception e) {
                        output.Message = e.Message;
                    }
                }
                break;
            }
        }
    }
}
