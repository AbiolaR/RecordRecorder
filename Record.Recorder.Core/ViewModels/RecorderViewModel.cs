using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

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
        public ICommand PressPauseCommand { get; set; }
        public ICommand PressStopCommand { get; set; }

        private bool isRecording, isRecordingInProgress = false;

        public bool IsRecording { get => isRecording; set { isRecording = value; OnPropertyChanged(nameof(IsRecording)); } }
        public bool IsRecordingInProgress { get => isRecordingInProgress; set { isRecordingInProgress = value; OnPropertyChanged(nameof(IsRecordingInProgress)); } }

        private string currentRecordingTime = "00:00:00";
        private readonly DispatcherTimer timer;
        private readonly Stopwatch stopwatch = new Stopwatch();

        public string CurrentRecordingTime { get => currentRecordingTime; set { currentRecordingTime = value; OnPropertyChanged(nameof(CurrentRecordingTime)); } }

        public RecorderViewModel()
        {
            GoToSettingsCommand = new RelayCommand((o) => SetCurrentPageTo(ApplicationPage.SettingsPage));
            PressRecordCommand = new RelayCommand((o) => StartRecording());
            PressStopCommand = new RelayCommand((o) => StopRecording());
            PressPauseCommand = new RelayCommand((o) => ToggleIsRecording());            

            timer = new DispatcherTimer(DispatcherPriority.Render);
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (s, a) =>
            {
                if (stopwatch.IsRunning)
                {
                    TimeSpan timeSpan = stopwatch.Elapsed;
                    CurrentRecordingTime = string.Format("{0:00}:{1:00}:{2:00}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
                }
            };
        }



        private void StopRecording()
        {
            IsRecording = false;
            IsRecordingInProgress = false;
            stopwatch.Stop();
            timer.Stop();
            stopwatch.Reset();
        }

        private async void StartRecording()
        {
            CurrentRecordingTime = "00:00:00";
            await Task.Delay(500);
            IsRecording = true;
            IsRecordingInProgress = true;
            stopwatch.Start();
            timer.Start();
        }

        private void ToggleIsRecording()
        {
            //IsRecording = !IsRecording;
            ToggleCommmand(() => IsRecording, () => { stopwatch.Stop(); timer.Stop(); }, () => { stopwatch.Start(); timer.Start(); });
        }
    }
}
