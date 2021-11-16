using Record.Recorder.Core;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace RecordRecorder
{
    /// <summary>
    /// The applications implementation of the <see cref="IUIManager"/>
    /// </summary>
    public class UIManager : IUIManager
    {

        /// <summary>
        /// Displays a message box to the user
        /// </summary>
        /// <param name="viewModel">The view model</param>
        /// <returns></returns>
        public Task ShowMessage(MessageBoxDialogViewModel viewModel)
        {
            return new DialogMessageBox().ShowDialog(viewModel);
            //return Task.FromResult(0);
        }

        public Task ShowMessageWithOption(MessageBoxButtonDialogViewModel viewModel)
        {
            return new DialogMessageOptionBox().ShowDialog(viewModel);
        }


        public Task<string> ChooseFolderLocation()
        {
            var tcs = new TaskCompletionSource<string>();
            var path = "";

            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    using var dialog = new FolderBrowserDialog();
                    if (DialogResult.OK.Equals(dialog.ShowDialog()))
                    {
                        path = dialog.SelectedPath;
                        //SaveOutputFolderLocation(dialog.SelectedPath);
                    }
                }
                finally
                {
                    tcs.TrySetResult(path);
                }

            });

            return tcs.Task;
        }

        public Task OpenFolderLocation(string path)
        {
            var tcs = new TaskCompletionSource<bool>();

            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    Process.Start("explorer.exe", path);
                }
                finally
                {
                    tcs.TrySetResult(true);
                }

            });

            return tcs.Task;
        }

        public Task ShowProgressDialogWithOption(ProgressBoxDialogViewModel viewModel)
        {
            return new DialogSavingProgressBox().ShowDialog(viewModel);
        }

        public Task OpenBrowserWindow()
        {
            var tcs = new TaskCompletionSource<bool>();

            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    new BrowserWindow().ShowDialog();
                }
                finally
                {
                    tcs.TrySetResult(true);
                }

            });

            return tcs.Task;
        }
    }
}
