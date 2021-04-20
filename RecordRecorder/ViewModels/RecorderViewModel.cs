using System;
using System.Collections.Generic;
using System.Text;

namespace RecordRecorder.Recorder.ViewModels
{
    /// <summary>
    /// A view model for the Recorder
    /// </summary>
    class RecorderViewModel : BaseViewModel
    {
        public Dictionary<int, string> RecordingDevices { get; set; }

        public int SelectedRecordingDevice { get; set; }
    }
}
