using Record.Recorder.Core;

namespace RecordRecorder
{

    /// <summary>
    /// Locates view models from the IoC for use in binding in Xaml files
    /// </summary>
    public class ViewModelLocator
    {
        #region Public Properties

        /// <summary>
        /// Singleton instance of the locator
        /// </summary>
        public static ViewModelLocator Instance { get; private set; } = new ViewModelLocator();

        
        /// <summary>
        /// The application view model
        /// </summary>
        public ApplicationViewModel ApplicationViewModel => IoC.Get<ApplicationViewModel>();

        /*
        /// <summary>
        /// The settings view model
        /// </summary>
        public SettingsViewModel SettingsViewModel => ViewModelSettings;

        public WindowViewModel ViewModelWindow { get; private set; }
        public SettingsViewModel ViewModelSettings { get; private set; }
        */

        #endregion
        
    }
}
