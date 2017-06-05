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
using Windows.UI.Popups;
using RPNCalc.Dialogs;
using RPNCalc.Controls;

namespace RPNCalc {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FunctionsPage : Page {
        private MainPage mPage;

        public FunctionsPage() {
            InitializeComponent();
            var mgr = (Application.Current as App).CalcManager;
            lvFuncs.ItemsSource = mgr.Functions;
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            mPage = e.Parameter as MainPage;
        }

        private void OnDoubleClick(object sender, DoubleTappedRoutedEventArgs e) {
            mPage.InsertFunction(lvFuncs.SelectedItem as CalcManager.Function);
            lvFuncs.SelectedIndex = -1;
            Frame.GoBack();
        }

        private void OnAdded(object sender, RoutedEventArgs e) {
            var fn = new CalcManager.Function() {
                Name = null,
                Descr = null,
                ParamNum = 0,
                Code = new CalcCommand[0],
            };
            mPage.EditFunction = fn;
            lvFuncs.SelectedIndex = -1;
            Frame.GoBack();
        }

        private void OnEdited(object sender, RoutedEventArgs e) {
            if(lvFuncs.SelectedItem == null) return;
            mPage.EditFunction = lvFuncs.SelectedItem as CalcManager.Function;
            lvFuncs.SelectedIndex = -1;
            Frame.GoBack();
        }

        private async void OnDeleted(object sender, RoutedEventArgs e) {
            if(lvFuncs.SelectedItem == null) return;
            var fn = lvFuncs.SelectedItem as CalcManager.Function;

            var dialog = new MessageDialog($"Are you sure you want to delete '{fn.Name}'?", "Confirm");
            dialog.Commands.Add(new UICommand("Yes") { Id = true });
            dialog.Commands.Add(new UICommand("No") { Id = false });
            var result = await dialog.ShowAsync();
            if(!(bool)result.Id) return;

            var app = (Application.Current as App);
            app.CalcManager.Functions.Remove(fn);
            app.Calculator.RemoveUserFunc(fn.Name);
            lvFuncs.SelectedIndex = -1;
        }
    }
}
