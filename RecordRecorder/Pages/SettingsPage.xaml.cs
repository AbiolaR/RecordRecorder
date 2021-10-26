using Record.Recorder.Core;
using System.Windows;

namespace RecordRecorder
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : BasePage<SettingsViewModel>
    {
        public SettingsPage()
        {
            InitializeComponent();
            var recorder = new RecorderUtil();
            var test = recorder.GetRecordingDevices();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
           
        }
    }
}
