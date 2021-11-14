using System;
using System.ComponentModel;
using System.IO;

namespace Record.Recorder.Core
{
    public class ProgressBoxDialogViewModel : MessageBoxButtonDialogViewModel
    {
        public virtual BackgroundWorker BGWorker { get; }
        private readonly RecorderUtil recorder = new RecorderUtil();

        private double progressValue = 0;
        public double ProgressValue { get => (int)progressValue; set { progressValue = value; OnPropertyChanged(nameof(ProgressValue)); } }

        bool isRecordingSaved = false;
        public bool IsRecordingSaved { get => isRecordingSaved; set { isRecordingSaved = value; OnPropertyChanged(nameof(IsRecordingSaved)); } }

        public ProgressBoxDialogViewModel()
        {
            BGWorker = new BackgroundWorker();
            ProgressValue = 0;
            BGWorker.ProgressChanged += new ProgressChangedEventHandler(ProgressChanged);
            BGWorker.DoWork += new DoWorkEventHandler(StartSaving);
            BGWorker.WorkerReportsProgress = true;
            BGWorker.WorkerSupportsCancellation = true;
        }

        public override void OnDialogOpen()
        {
            IsRecordingSaved = false;
            ProgressValue = 0;
            BGWorker.RunWorkerAsync();
        }

        private async void StartSaving(object sender, DoWorkEventArgs e)
        {
            IoC.Settings.SongDetectionType = Type.SongDetectionType.SPOTIFY;
            await recorder.DetectAndSaveTracksAsync(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), @"Test\Resources\full12min.wav"));// @"C:\Users\rasheed_abiola\source\repos\RecordRecorder\Record.Recorder.Core.UnitTests\Resources\Audio\full12min.wav");
            BGWorker.CancelAsync();
            BGWorker.Dispose();
            ProgressValue = 100;
            IsRecordingSaved = true;
            Message = "Done!";
        }

        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            double newVal = progressValue + 1d / (double)e.ProgressPercentage * (double)e.UserState * 100;
            if (newVal >= 100)
            {
                ProgressValue = 100;
            }
            else
            {
                ProgressValue = newVal;
            }
        }
    }

}
