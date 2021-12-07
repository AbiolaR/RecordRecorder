using NLog;
using Record.Recorder.Core;
using Record.Recorder.Type;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace RecordRecorder
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static Version Version { get; } = new Version("1.1.4");

        readonly Logger log = LogManager.GetLogger("fileLogger");
        
        
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
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(AppUnhandledException);
            // Setup IoC
            IoC.Setup();

            // Bind an update manager
            IoC.Kernel.Bind<IUpdateManager>().ToConstant(new UpdateManager());

            // Bind a UI manager
            IoC.Kernel.Bind<IUIManager>().ToConstant(new UIManager());

            // Bind a settings manager
            IoC.Kernel.Bind<ISettingsManager>().ToConstant(new SettingsManager());

            // Set necessary default settings data
            if (string.IsNullOrEmpty(IoC.Settings.OutputFolderLocation))
            {
                IoC.Settings.OutputFolderLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), Assembly.GetEntryAssembly().GetName().Name);
            }
            if (string.IsNullOrEmpty(IoC.Settings.ApplicationLanguage))
            {
                IoC.Settings.ApplicationLanguage = Thread.CurrentThread.CurrentCulture.ToString() switch
                {
                    "de" => "Deutsch",
                    "de-DE" => "Deutsch",
                    _ => "English",
                };
            }

            // Reset album name at startup
            IoC.Settings.AlbumName = "";

            // Set Theme
            switch (IoC.Settings.ApplicationTheme)
            {
                case ApplicationTheme.DARK:
                    IoC.ApplicationVM.SetThemeToDark();
                    break;

                default:
                    IoC.ApplicationVM.SetThemeToLight();
                    break;
            }

            SetupLanguage();

            CheckForUpdate();
        }

        private void AppUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            log.Fatal(e.ExceptionObject.ToString());
        }

        private void SetupLanguage()
        {
            string language = IoC.Settings.ApplicationLanguage;
            switch (language)
            {
                case "Deutsch":
                    Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("de");
                    break;
            }
        }

        private async void CheckForUpdate()
        {
            bool updateAvailable = await IoC.UpdateManager.CheckForUpdateAvailableAsync();
            if (updateAvailable)
            {
                var viewModel = new MessageBoxButtonDialogViewModel()
                {
                    Title = Text.UpdateAvailable,
                    Message = Text.UpdateAvailableMessage,
                    OkText = Text.No,
                    ButtonText = Text.Yes
                };
                await IoC.UI.ShowMessageWithOption(viewModel);
                if (viewModel.Answer == DialogAnswer.Option1)
                {
                    var progressViewModel = IoC.SavingProgressVM;
                    progressViewModel.Title = Text.DownloadingUpdate;
                    progressViewModel.Message = Text.DownloadingUpdateMessage;
                    progressViewModel.SetTask(() => IoC.UpdateManager.DownloadUpdate());
                    progressViewModel.CloseWhenDone = true;

                    await IoC.UI.ShowProgressDialog(progressViewModel);
                    IoC.UpdateManager.UpdateApplication();
                    Application.Current.Shutdown();
                }
            }
        }
    }
}
