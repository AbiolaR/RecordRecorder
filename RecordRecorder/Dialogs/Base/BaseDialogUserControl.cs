using Record.Recorder.Core;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RecordRecorder
{
    /// <summary>
    /// The base class for any content that is being used inside of a <see cref="DialogWindow"/>
    /// </summary>
    public class BaseDialogUserControl : UserControl
    {
        #region Private Properties

        private readonly DialogWindow dialogWindow;

        #endregion

        #region Public Properties

        #endregion

        #region Public Commands

        public ICommand CloseCommand { get; set; }
        public ICommand OptionalCommand { get; set; }

        public int WindowMinimumWidth { get; set; } = 250;

        public int WindowMinimumHeight { get; set; } = 100;

        public int TitleHeight { get; set; } = 20;

        public string Title { get; set; }


        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public BaseDialogUserControl()
        {
            dialogWindow = new DialogWindow();
            dialogWindow.ViewModel = new DialogWindowViewModel(dialogWindow);

            
        }

        #endregion

        #region Public Dialog Show Methods

        public Task ShowDialog<T>(T viewModel)
            where T : BaseDialogViewModel
        {
            var tcs = new TaskCompletionSource<bool>();

            Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    dialogWindow.Owner = Application.Current.MainWindow;

                    dialogWindow.ViewModel.WindowMinimumWidth = WindowMinimumWidth;
                    dialogWindow.ViewModel.WindowMinimumHeight = WindowMinimumHeight;
                    dialogWindow.ViewModel.TitleHeight = TitleHeight;
                    dialogWindow.ViewModel.Title = string.IsNullOrEmpty(viewModel.Title) ? Title : viewModel.Title;

                    dialogWindow.Content = this;
                    dialogWindow.ContentViewModel = viewModel;

                    DataContext = viewModel;
                    CloseCommand = new RelayCommand((o) => CloseDialog(viewModel, o));

                    if (typeof(ProgressBoxDialogViewModel).IsInstanceOfType(viewModel))
                    {
                        (viewModel as ProgressBoxDialogViewModel).OnTaskDone += (s, e) => {
                            CloseDialog(viewModel, null);
                        };
                    }

                    IoC.Get<ApplicationViewModel>().IsFocused = false;
                    dialogWindow.ShowDialog();
                }
                finally
                {
                    tcs.TrySetResult(true);
                }

            });

            return tcs.Task;
        }

        #endregion


        #region Private Methods

        private void CloseDialog<T>(T viewModel, object o)
            where T : BaseDialogViewModel
        {
            var tcs = new TaskCompletionSource<bool>();

            Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    dialogWindow.Close();
                    IoC.Get<ApplicationViewModel>().IsFocused = true;

                    if (o != null)
                        viewModel.Answer = (DialogAnswer)o;
                }
                finally
                {
                    tcs.TrySetResult(true);
                }
            });
        } 

        #endregion
    }


}
