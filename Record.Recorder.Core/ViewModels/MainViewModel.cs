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
        public ICommand SaveRecordingCommand { get; set; }

        private static readonly string ZEROTIMERVALUE = "00:00:00";

        private bool isRecording, isRecordingInProgress, isRecordingAllowed = false, isRecordingSaved = true;

        public bool IsRecording { get => isRecording; set { isRecording = value; OnPropertyChanged(nameof(IsRecording)); } }
        public bool IsRecordingInProgress { get => isRecordingInProgress; set { isRecordingInProgress = value; OnPropertyChanged(nameof(IsRecordingInProgress)); } }
        public bool IsRecordingAllowed { get => isRecordingAllowed; set { isRecordingAllowed = value; OnPropertyChanged(nameof(IsRecordingAllowed)); } }
        public bool IsRecordingSaved { get => isRecordingSaved; set { isRecordingSaved = value; OnPropertyChanged(nameof(IsRecordingSaved)); } }

        public string AlbumName { get => IoC.Settings.AlbumName; set => IoC.Settings.AlbumName = value; }

        private double loadingVal = 0;
        public double LoadingVal { get => ((int)loadingVal); set { loadingVal = value; OnPropertyChanged(nameof(LoadingVal)); } }
        public virtual BackgroundWorker BGWorker { get; }

        private string currentRecordingTime = ZEROTIMERVALUE;
        private readonly DispatcherTimer timer;
        private readonly Stopwatch stopwatch = new Stopwatch();

        public string CurrentRecordingTime { get => currentRecordingTime; set { currentRecordingTime = value; OnPropertyChanged(nameof(CurrentRecordingTime)); } }


        private readonly RecorderUtil recorder = new RecorderUtil();


        public MainViewModel()
        {
            GoToSettingsCommand = new RelayCommand((o) => NavigateToSettingsPage());
            PressRecordCommand = new RelayCommand((o) => ToggleIsRecordingSaved());
            PressStopCommand = new RelayCommand((o) => StopRecording());
            PressPauseCommand = new RelayCommand((o) => ToggleIsRecording());
            SaveRecordingCommand = new RelayCommand((o) => SaveRecording());

            BGWorker = new BackgroundWorker();
            BGWorker.ProgressChanged += new ProgressChangedEventHandler(ProgressChanged);
            BGWorker.DoWork += new DoWorkEventHandler(StartSaving);
            BGWorker.WorkerReportsProgress = true;
            BGWorker.WorkerSupportsCancellation = true;

            timer = new DispatcherTimer(DispatcherPriority.Render)
            {
                Interval = TimeSpan.FromSeconds(1)
            };
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

        private async void StartRecording()
        {
            LoadingVal = 0;
            if (await CheckForRecordingDevice())
            {
                IsRecordingAllowed = true;
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

        private void ToggleIsRecordingSaved()
        {
            ToggleCommmand(() => IsRecordingSaved, () => StartRecording(), () => SaveRecording());
        }

        private void StopRecording()
        {
            IsRecording = false;
            IsRecordingInProgress = false;
            IoC.Get<ApplicationViewModel>().IsRecordingInProgress = false;
            IsRecordingAllowed = false;
            IsRecordingSaved = false;
            stopwatch.Stop();
            timer.Stop();
            stopwatch.Reset();
        }

        private async void SaveRecording()
        {

            var viewModel = IoC.SavingProgressVM;
            viewModel.Title = "Saving songs";
            viewModel.Message = "Songs are being detected...";
            viewModel.OkText = "Close";
            viewModel.ButtonText = "Open Folder";
            
            await IoC.UI.ShowProgressDialogWithOption(viewModel);
            if (viewModel.Answer == DialogAnswer.Option1)
            {
                await IoC.UI.OpenFolderLocation(viewModel.OutputFolder);
            }

        }

        private async void StartSaving(object sender, DoWorkEventArgs e)
        {
            await recorder.DetectAndSaveTracksAsync(@"C:\Users\rasheed_abiola\source\repos\RecordRecorder\Record.Recorder.Core.UnitTests\Resources\Audio\full12min.wav");
            BGWorker.CancelAsync();
            BGWorker.Dispose();
            LoadingVal = 100;
            CurrentRecordingTime = ZEROTIMERVALUE;
        }

        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            double newVal = loadingVal + 1d / (double)e.ProgressPercentage * (double)e.UserState * 100;
            if (newVal >= 100)
            {
                LoadingVal = 100;
            } else
            {
                LoadingVal = newVal;
            }
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
