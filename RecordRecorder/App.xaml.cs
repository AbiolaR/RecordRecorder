using Record.Recorder.Core;
using System.Threading;
using System.Windows;

namespace RecordRecorder
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Custom startup so the IoC is loaded immediately before anything else
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            // let the base application do what it needs
            base.OnStartup(e);

            ApplicationSetup();


            // Show the main window
            Current.MainWindow = new MainWindow();
            Current.MainWindow.Show();
        }

        // Configures the application reafy for use
        private void ApplicationSetup()
        {
            // Setup IoC
            IoC.Setup();

            // Bind a UI manager
            IoC.Kernel.Bind<IUIManager>().ToConstant(new UIManager());

            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("de");

            //SetupLanguageDictionary();

        }

        private void SetupLanguageDictionary()
        {
            var dictionary = new ResourceDictionary{ Source = new System.Uri(@"..\Resources\Languages\english.xaml", System.UriKind.Relative) };
            
            switch (Thread.CurrentThread.CurrentCulture.ToString())
            {
                case "de":
                    Resources.MergedDictionaries.RemoveAt(0);
                    dictionary.Source = new System.Uri(@"..\Resources\Languages\german.xaml", System.UriKind.Relative);
                    Resources.MergedDictionaries.Add(dictionary);
                    break;

                /*default:
                    //dictionary.Source = new System.Uri(@"..\Resources\Languages\english.xaml", System.UriKind.Relative);
                    break;*/

            }
        }

    }
}
