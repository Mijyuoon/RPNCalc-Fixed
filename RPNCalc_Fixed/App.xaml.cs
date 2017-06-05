using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Core;
using Windows.UI.Popups;
using RPNCalc.Dialogs;
using RPNCalcLib;

namespace RPNCalc {
    sealed partial class App : Application {
        private Calculator calcul;
        private CalcManager calcmgr;

        public Calculator Calculator {
            get { return calcul; }
        }
        public CalcManager CalcManager {
            get { return calcmgr; }
        }

        public const string FILE_FUNCTIONS_MAIN = "functions.dat";
        public const string FILE_FUNCTIONS_BACKUP = "functions.bak";

        public App() {
            InitializeComponent();
            Suspending += OnSuspending;

            calcul = new Calculator();
            calcmgr = new CalcManager(calcul);

            CalcDefaults.BasicOperators(calcul);
            CalcDefaults.AdvancedOperators(calcul);
            CalcDefaults.Programming(calcul);
            CalcDefaults.Constants(calcul);
            CalcDefaults.Miscelaneous(calcul);
        }

        public async Task SaveFunctionTable(string fname) {
            var localFolder = ApplicationData.Current.LocalFolder;
            var file = await localFolder.CreateFileAsync(fname, CreationCollisionOption.ReplaceExisting);
            using(var stream = await file.OpenStreamForWriteAsync()) {
                var writer = new BinaryWriter(stream);
                writer.Write(calcmgr.Functions.Count);
                foreach(var func in calcmgr.Functions)
                    func.Serialize(writer);
                await stream.FlushAsync();
            }
        }

        public async Task LoadFunctionTable(string fname) {
            var localFolder = ApplicationData.Current.LocalFolder;
            var stored = await localFolder.TryGetItemAsync(fname);
            if(stored != null) {
                try {
                    calcmgr.ClearAllFunctions();
                    var file = stored as StorageFile;
                    using(var stream = await file.OpenStreamForReadAsync()) {
                        var reader = new BinaryReader(stream);
                        var count = reader.ReadInt32();
                        for(int i = 0; i < count; i++) {
                            var func = CalcManager.Function.Deserialize(reader);
                            var cfunc = func.Compile(calcul);
                            calcmgr.Functions.Add(func);
                            calcul.SetUserFunc(func.Name, cfunc);
                        }
                    }
                } catch(EndOfStreamException) {
                    await stored.DeleteAsync();
                    calcmgr.ClearAllFunctions();
                    var msgbox = new MessageDialog("Function storage corrupted", "Error");
                    await msgbox.ShowAsync();
                }
            }
        }

        protected async override void OnLaunched(LaunchActivatedEventArgs e) {
#if DEBUG
            if(System.Diagnostics.Debugger.IsAttached) {
                DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if(rootFrame == null) {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;
                rootFrame.Navigated += OnNavigated;

                /*if(e.PreviousExecutionState == ApplicationExecutionState.Terminated
                || e.PreviousExecutionState == ApplicationExecutionState.ClosedByUser) {
                    LoadFunctionTable();
                }*/
                if(calcmgr.Functions.Count < 1)
                    await LoadFunctionTable(FILE_FUNCTIONS_MAIN);

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;

                var navmgr = SystemNavigationManager.GetForCurrentView();
                navmgr.BackRequested += OnBackRequested;
                OnNavigated(rootFrame, null);
            }

            if(rootFrame.Content == null) {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                rootFrame.Navigate(typeof(MainPage), e.Arguments);
            }
            // Ensure the current window is active
            Window.Current.Activate();
        }

        private void OnNavigated(object sender, NavigationEventArgs e) {
            var navmgr = SystemNavigationManager.GetForCurrentView();
            navmgr.AppViewBackButtonVisibility = (sender as Frame).CanGoBack ? AppViewBackButtonVisibility.Visible
                                                                             : AppViewBackButtonVisibility.Collapsed;
        }

        void OnNavigationFailed(object sender, NavigationFailedEventArgs e) {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        private async void OnSuspending(object sender, SuspendingEventArgs e) {
            var deferral = e.SuspendingOperation.GetDeferral();
            //if(calcmgr.FunctionsChanged)
            await SaveFunctionTable(FILE_FUNCTIONS_MAIN);
            deferral.Complete();
        }

        private void OnBackRequested(object sender, BackRequestedEventArgs e) {
            Frame rootFrame = Window.Current.Content as Frame;
            if(rootFrame.CanGoBack) {
                rootFrame.GoBack();
                e.Handled = true;
            }
        }
    }
}
