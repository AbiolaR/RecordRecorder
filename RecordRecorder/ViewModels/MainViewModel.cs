using Record.Recorder.Core;
using System.Windows;

namespace RecordRecorder
{
    public class MainViewModel : BaseMainViewModel
    {
        protected override void ShowNoRecordingDeviceAlert()
        {
            MessageBox.Show("No recording device has been saved.\nPlease choose one and make sure it works in the settings.", "Recording Device Not Set");
        }

        protected override void ShowRecordingDeviceNotFoundAlert()
        {
            MessageBox.Show("The saved recording device could not be found.\nPlease make sure it is plugged in or choose one in the settings.", "Recording Device Not Found");
        }
    }
}
