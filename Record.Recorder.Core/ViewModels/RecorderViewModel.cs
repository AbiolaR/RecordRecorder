using System.Windows;
using System.Windows.Input;

namespace Record.Recorder.Core
{
    /// <summary>
    /// A view model for the Recorder
    /// </summary>
    public class RecorderViewModel : BaseViewModel
    {
        /// <summary>
        /// The command to switch to the settings page
        /// </summary>
        public ICommand GoToSettingsCommand { get; set; }
        public ICommand PressRecordCommand { get; set; }

        private bool isRecording = false;

        public bool IsRecording { get => isRecording; set { isRecording = value; OnPropertyChanged(nameof(IsRecording)); } }


        public RecorderViewModel()
        {
            GoToSettingsCommand = new RelayCommand((o) => SetCurrentPageTo(ApplicationPage.SettingsPage));
            PressRecordCommand = new RelayCommand((o) => ToggleIsRecording());
        }

        private void ToggleIsRecording()
        {
            IsRecording = !IsRecording;
        }
    }
}
