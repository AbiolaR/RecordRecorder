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
    public class MainViewModel : BaseViewModel
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

        public MainViewModel()
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



        private void NavigateToSettingsPage()
        {
            if (IsRecordingInProgress)
            {
                ShowRecordingInProgressDialog();                
                return;
            }

            SetCurrentPageTo(ApplicationPage.SettingsPage);
        }

        private void StopRecording()
        {
            IsRecording = false;
            IsRecordingInProgress = false;
            IoC.Get<ApplicationViewModel>().IsRecordingInProgress = false;
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
                IoC.Get<ApplicationViewModel>().IsRecordingInProgress = true;
                stopwatch.Start();
                timer.Start();
            }
        }

        private void ToggleIsRecording()
        {
            ToggleCommmand(() => IsRecording, () => { stopwatch.Stop(); timer.Stop(); }, () => { stopwatch.Start(); timer.Start(); });
        }

        private async Task<bool> CheckForRecordingDevice()
        {
            string recordingDeviceName = Properties.Settings.Default["RecordingDevice"].ToString();

            if (string.IsNullOrEmpty(recordingDeviceName))
            {
                ShowNoRecordingDeviceDialog();
                return false;
            }

            var recordingDevice = await recorder.GetRecordingDeviceByName(recordingDeviceName);
            if (recordingDevice.Equals(default(KeyValuePair<int, string>)))
            {
                ShowRecordingDeviceNotFoundDialog();                
                return false;
            }
            return true;
        }

        #region Show Dialog Methods

        private async void ShowRecordingInProgressDialog()
        {
            await IoC.UI.ShowMessage(new MessageBoxDialogViewModel
            {
                Title = "Recording in Progress",
                Message = "You are currently recording.\nTo abort the recording click on Stop.",
                OkText = "OK"
            });
        }

        private async void ShowRecordingDeviceNotFoundDialog()
        {
            var viewModel = new MessageBoxButtonDialogViewModel
            {
                Title = "Recording Device Not Found",
                Message = "The saved recording device could not be found.\nPlease make sure it is plugged in or choose one in the settings.",
                OkText = "OK",
                ButtonText = "Settings"
            };

            await IoC.UI.ShowMessageWithOption(viewModel);

            if (viewModel.Answer == DialogAnswer.Option1)
            {
                NavigateToSettingsPage();
            }
        }

        private async void ShowNoRecordingDeviceDialog()
        {
            var viewModel = new MessageBoxDialogViewModel
            {
                Title = "Recording Device Not Set",
                Message = "No recording device has been saved.\nPlease choose one and make sure it works in the settings.",
                OkText = "Settings"
            };
            await IoC.UI.ShowMessage(viewModel);

            if (viewModel.Answer == DialogAnswer.OK)
            {
                NavigateToSettingsPage();
            }
        }

        #endregion
    }
}
