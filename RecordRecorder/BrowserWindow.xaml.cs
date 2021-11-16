using Record.Recorder.Core;
using System.ComponentModel;
using System.Windows;

namespace RecordRecorder
{
    /// <summary>
    /// Interaction logic for BrowserWindow.xaml
    /// </summary>
    public partial class BrowserWindow : Window
    {

        private DialogWindowViewModel viewModel;
        public DialogWindowViewModel ViewModel { get => viewModel; set { viewModel = value; DataContext = viewModel; } }

        private BaseDialogViewModel contentViewModel;
        public BaseDialogViewModel ContentViewModel { get => contentViewModel; set { contentViewModel = value; } }

        public BrowserWindow()
        {
            InitializeComponent();
            //AlbumBrowser.Navigate("https://theaudiodb.com/");
            //AlbumBrowser.Navigate("https://google.com");
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            IoC.Get<ApplicationViewModel>().IsFocused = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //ContentViewModel.OnDialogOpen();
        }
    }
}
