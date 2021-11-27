using Record.Recorder.Core;
using System.ComponentModel;
using System.Windows;

namespace RecordRecorder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();                       
            DataContext = new WindowViewModel(this);
        }

        protected async override void OnClosing(CancelEventArgs e)
        {
            if (IoC.Get<ApplicationViewModel>().IsRecordingInProgress)
            {
                e.Cancel = true;

                await IoC.UI.ShowMessage(new MessageBoxDialogViewModel
                {
                    Title = Text.RecordingInProgress,
                    Message = Text.RecordingInProgressMessage,
                    OkText = Text.OK
                });

            }            
        }
    }
}
