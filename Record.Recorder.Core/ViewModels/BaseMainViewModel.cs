using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace Record.Recorder.Core
{
    /// <summary>
    /// A view model for the Recorder
    /// </summary>
    public abstract class BaseMainViewModel : BaseViewModel
    {
        /// <summary>
        /// The command to switch to the settings page
        /// </summary>
        public ICommand GoToSettingsCommand { get; set; }
        public ICommand PressRecordCommand { get; set; }
        public ICommand PressPauseCommand { get; set; }
        public ICommand PressStopCommand { get; set; }

        private bool isRecording, isRecordingInProgress, isRecordingAllowed = false;

        public bool IsRecording { get => isRecording; set { isRecording = value; OnPropertyChanged(nameof(IsRecording)); } }
        public bool IsRecordingInProgress { get => isRecordingInProgress; set { isRecordingInProgress = value; OnPropertyChanged(nameof(IsRecordingInProgress)); } }
        public bool IsRecordingAllowed { get => isRecordingAllowed; set { isRecordingAllowed = value; OnPropertyChanged(nameof(IsRecordingAllowed)); } }

        private string currentRecordingTime = "00:00:00";
        private readonly DispatcherTimer timer;
        private readonly Stopwatch stopwatch = new Stopwatch();

        private readonly RecorderUtil recorder = new RecorderUtil();


        public string CurrentRecordingTime { get => currentRecordingTime; set { currentRecordingTime = value; OnPropertyChanged(nameof(CurrentRecordingTime)); } }

        public BaseMainViewModel()
        {
            GoToSettingsCommand = new RelayCommand((o) => NavigateToSettingsPage());
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


        protected abstract void ShowNoRecordingDeviceAlert();
        protected abstract void ShowRecordingDeviceNotFoundAlert();
        protected abstract void RecordingInProgressAlert();

        private void NewRecordingInProgressAlert()
        {
            IoC.UI.ShowMessage(new MessageBoxDialogViewModel
            {
                Title = "Recording in Progress",
                Message = "You are currently recording.\nTo abort the recording click on Stop.",
                OkText = "OK"
            });
        }

        private void NavigateToSettingsPage()
        {
            if (IsRecordingInProgress)
            {
                NewRecordingInProgressAlert();
                return;
            }

            SetCurrentPageTo(ApplicationPage.SettingsPage);
        }

        private void StopRecording()
        {
            IsRecording = false;
            IsRecordingInProgress = false;
            IsRecordingAllowed = false;
            stopwatch.Stop();
            timer.Stop();
            stopwatch.Reset();
        }

        private async void StartRecording()
        {
            if (await CheckForRecordingDevice())
            {
                IsRecordingAllowed = true;
                CurrentRecordingTime = "00:00:00";
                await Task.Delay(500);
                IsRecording = true;
                IsRecordingInProgress = true;
                stopwatch.Start();
                timer.Start();
            }
        }

        private void ToggleIsRecording()
        {
            //IsRecording = !IsRecording;
            ToggleCommmand(() => IsRecording, () => { stopwatch.Stop(); timer.Stop(); }, () => { stopwatch.Start(); timer.Start(); });
        }

        private async Task<bool> CheckForRecordingDevice()
        {
            string recordingDeviceName = Properties.Settings.Default["recordingDevice"].ToString();

            if (string.IsNullOrEmpty(recordingDeviceName))
            {
                ShowNoRecordingDeviceAlert();
                return false;
            }

            var recordingDevice = await recorder.GetRecordingDeviceByName(recordingDeviceName);
            if (recordingDevice.Equals(default(KeyValuePair<int, string>)))
            {
                ShowRecordingDeviceNotFoundAlert();
                return false;
            }
            return true;
        }
    }
}
