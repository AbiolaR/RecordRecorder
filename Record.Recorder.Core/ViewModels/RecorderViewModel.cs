using System.Collections.Generic;
using System.Windows.Input;

namespace Record.Recorder.Core
{
    /// <summary>
    /// A view model for the Recorder
    /// </summary>
    public class RecorderViewModel : BaseViewModel
    {
        public Dictionary<int, string> RecordingDevices { get; set; }

        public int SelectedRecordingDevice { get; set; }

        /// <summary>
        /// The command to switch to the settings page
        /// </summary>
        public ICommand GoToSettingsCommand { get; set; }

        public RecorderViewModel()
        {
            GoToSettingsCommand = new RelayCommand((o) => SetCurrentPageTo(ApplicationPage.SettingsPage));
        }
    }
}
