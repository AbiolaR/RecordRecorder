using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        private static readonly string ALBUMNAME = "AlbumName";
        private static readonly string RECORDINGDEVICE = "RecordingDevice";
        private static readonly string ZEROTIMERVALUE = "00:00:00";

        private bool isRecording, isRecordingInProgress, isRecordingAllowed = false;

        public bool IsRecording { get => isRecording; set { isRecording = value; OnPropertyChanged(nameof(IsRecording)); } }
        public bool IsRecordingInProgress { get => isRecordingInProgress; set { isRecordingInProgress = value; OnPropertyChanged(nameof(IsRecordingInProgress)); } }
        public bool IsRecordingAllowed { get => isRecordingAllowed; set { isRecordingAllowed = value; OnPropertyChanged(nameof(IsRecordingAllowed)); } }

        public string AlbumName { get => IoC.Settings.AlbumName; set => IoC.Settings.AlbumName = value; }

        private double loadingVal = 0;
        public double LoadingVal { get => loadingVal; set { loadingVal = value; OnPropertyChanged(nameof(LoadingVal)); } }

        private string currentRecordingTime = ZEROTIMERVALUE;
        private readonly DispatcherTimer timer;
        private readonly Stopwatch stopwatch = new Stopwatch();

        private readonly RecorderUtil recorder = new RecorderUtil();

        public virtual BackgroundWorker BGWorker { get; }


        public string CurrentRecordingTime { get => currentRecordingTime; set { currentRecordingTime = value; OnPropertyChanged(nameof(CurrentRecordingTime)); } }

        public MainViewModel()
        {
            GoToSettingsCommand = new RelayCommand((o) => NavigateToSettingsPage());
            PressRecordCommand = new RelayCommand((o) => StartRecording());
            PressStopCommand = new RelayCommand((o) => StopRecording());
            PressPauseCommand = new RelayCommand((o) => ToggleIsRecording());

            BGWorker = new BackgroundWorker();

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
            BGWorker.ProgressChanged += new ProgressChangedEventHandler(progChanged);
            BGWorker.DoWork += new DoWorkEventHandler(startSaving);
            BGWorker.WorkerReportsProgress = true;
            BGWorker.RunWorkerAsync();
            //progress2.RunWorkerAsync();
            //progress.ProgressChanged += OnProgressReported;
            //IncreaseLoadingValue(1, 5);
            /*Task task = recorder.DetectAndSaveTracksAsync(@"C:\Users\rasheed_abiola\source\repos\RecordRecorder\Record.Recorder.Core.UnitTests\Resources\Audio\full12min.wav", progress, progress2);

            await task;*/
            /*if (await CheckForRecordingDevice())
            {
                IsRecordingAllowed = true;
                CurrentRecordingTime = ZEROTIMERVALUE;
                await Task.Delay(500);
                IsRecording = true;
                IsRecordingInProgress = true;
                IoC.Get<ApplicationViewModel>().IsRecordingInProgress = true;
                stopwatch.Start();
                timer.Start();
            }*/
        }

        private async void startSaving(object sender, DoWorkEventArgs e)
        {
            await recorder.DetectAndSaveTracksAsync(@"C:\Users\rasheed_abiola\source\repos\RecordRecorder\Record.Recorder.Core.UnitTests\Resources\Audio\full12min.wav");
        }

        private void progChanged(object sender, ProgressChangedEventArgs e)
        {
            LoadingVal += 1d / (double)e.ProgressPercentage * (double)e.UserState * 100;
        }

        private void OnProgressReported(object sender, Dictionary<string, double> e)
        {
            //Console.WriteLine(sender);
            //Console.WriteLine(e);

            e.TryGetValue("Weight", out double weight);
            e.TryGetValue("Denominator", out double denominator);

            LoadingVal += 1 / denominator * weight;
        }

        private void ToggleIsRecording()
        {
            ToggleCommmand(() => IsRecording, () => { stopwatch.Stop(); timer.Stop(); }, () => { stopwatch.Start(); timer.Start(); });
        }

        private async Task<bool> CheckForRecordingDevice()
        {
            string recordingDeviceName = IoC.Settings.RecordingDeviceName;

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

        public void IncreaseLoadingValue(double weight, double denominator)
        {
            LoadingVal += 1 / denominator * weight;
        }

        #region Show Dialog Methods

        private async void ShowRecordingInProgressDialog()
        {
            await IoC.UI.ShowMessage(new MessageBoxDialogViewModel
            {
                Title = Text.RecordingInProgress,
                Message = Text.RecordingInProgressMessage,
                OkText = Text.OK
            });
        }

        private async void ShowRecordingDeviceNotFoundDialog()
        {
            var viewModel = new MessageBoxButtonDialogViewModel
            {
                Title = Text.RecordingDeviceNotFound,
                Message = Text.RecordingDeviceNotFoundMessage,
                OkText = Text.OK,
                ButtonText = Text.Settings
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
                Title = Text.RecordingDeviceNotSet,
                Message = Text.RecordingDeviceNotSetMessage,
                OkText = Text.Settings
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
