using Record.Recorder.Core;
using System.ComponentModel;
using System.Windows;

namespace RecordRecorder
{
    /// <summary>
    /// Interaction logic for DialogWindow.xaml
    /// </summary>
    public partial class DialogWindow : Window
    {

        private DialogWindowViewModel viewModel;

        public DialogWindowViewModel ViewModel { get => viewModel; set { viewModel = value; DataContext = viewModel; } }

        public DialogWindow()
        {
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            IoC.Get<ApplicationViewModel>().IsFocused = true;
        }
    }
}
