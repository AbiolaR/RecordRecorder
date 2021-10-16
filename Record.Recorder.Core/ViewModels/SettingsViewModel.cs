using System.Windows.Input;

namespace Record.Recorder.Core
{
    public class SettingsViewModel : BaseViewModel
    {


        /// <summary>
        /// The command to switch to the main page
        /// </summary>
        public ICommand GoToHomeCommand { get; set; }

        public SettingsViewModel()
        {
            // Now some navigational commands
            GoToHomeCommand = new RelayCommand((o) => SetCurrentPageTo(ApplicationPage.MainPage));

        }

    }
}
