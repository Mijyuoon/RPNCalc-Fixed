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

namespace RPNCalc {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page {
        private CalcManager settings;

        public SettingsPage() {
            InitializeComponent();
            settings = (Application.Current as App).CalcManager;
            sFig.Value = settings.SignificantFigures;
            sEps.Value = settings.ComparisonEpsilon;
            sIter.Value = settings.IterationLimit;
            sRecur.Value = settings.RecursionLimit;
        }

        private void OnFiguresChanged(object sender, RangeBaseValueChangedEventArgs e) {
            if(e.OldValue != 0)
                settings.SignificantFigures = (int)e.NewValue;
        }

        private void OnEpsilonChanged(object sender, RangeBaseValueChangedEventArgs e) {
            if(e.OldValue != 0)
                settings.ComparisonEpsilon = e.NewValue;
        }

        private void OnIterLimitChanged(object sender, RangeBaseValueChangedEventArgs e) {
            if(e.OldValue != 0)
                settings.IterationLimit = (int)e.NewValue;
        }

        private void OnRecurLimitChanged(object sender, RangeBaseValueChangedEventArgs e) {
            if(e.OldValue != 0)
                settings.RecursionLimit = (int)e.NewValue;
        }

        private async void OnBackupClick(object sender, RoutedEventArgs e) {
            var app = (Application.Current as App);
            await app.SaveFunctionTable(App.FILE_FUNCTIONS_BACKUP);
        }

        private async void OnRestoreClick(object sender, RoutedEventArgs e) {
            var app = (Application.Current as App);
            await app.LoadFunctionTable(App.FILE_FUNCTIONS_BACKUP);
        }
    }
}
