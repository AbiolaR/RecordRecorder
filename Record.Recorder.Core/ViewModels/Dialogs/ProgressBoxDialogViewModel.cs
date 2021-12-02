using System;
using System.ComponentModel;

namespace Record.Recorder.Core
{
    public class ProgressBoxDialogViewModel : MessageBoxButtonDialogViewModel
    {
        public virtual BackgroundWorker BGWorker { get; }
        private Action _action;
         

        private double progressValue = 0;
        public double ProgressValue { get => (int)progressValue; set { progressValue = value; OnPropertyChanged(nameof(ProgressValue)); } }

        bool isTaskCompleted = false;
        public bool IsTaskCompleted { get => isTaskCompleted; set { isTaskCompleted = value; OnPropertyChanged(nameof(IsTaskCompleted)); } }

        public event EventHandler OnTaskDone;
        public bool CloseWhenDone { get; set; } = false;

        public object ReturnValue { get; set; }

        public ProgressBoxDialogViewModel()
        {
            BGWorker = new BackgroundWorker();
            ProgressValue = 0;
            BGWorker.ProgressChanged += new ProgressChangedEventHandler(ProgressChanged);
            BGWorker.DoWork += new DoWorkEventHandler(DoTask);
            BGWorker.WorkerReportsProgress = true;
            BGWorker.WorkerSupportsCancellation = true;
        }

        public override void OnDialogOpen()
        {
            IsTaskCompleted = false;
            ProgressValue = 0;
            BGWorker.RunWorkerAsync();
        }

        public void SetTask(Action action)
        {
            _action = action;
        }

        private void DoTask(object sender, DoWorkEventArgs e)
        {
            _action();
            
            BGWorker.CancelAsync();
            BGWorker.Dispose();
            ProgressValue = 100;
            if (CloseWhenDone)
            {
                OnTaskDone?.Invoke(this, new EventArgs());
            } else
            {
                IsTaskCompleted = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">First arg is the dneominator; Second the numerator</param>
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
