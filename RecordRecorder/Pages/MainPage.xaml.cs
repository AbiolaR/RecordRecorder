using Record.Recorder.Core;

namespace RecordRecorder
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : BasePage<MainViewModel>
    {
        public MainPage()
        {
            InitializeComponent();

            ViewModel = IoC.MainVM;
        }
    }
}
