using Record.Recorder.Core;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;

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
        }

        public Task ShowMessageWithOption(MessageBoxButtonDialogViewModel viewModel)
        {
            return new DialogMessageOptionBox().ShowDialog(viewModel);
        }


        public Task<string> ChooseFolderLocation()
        {
            var tcs = new TaskCompletionSource<string>();
            var path = "";

            Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    using var dialog = new System.Windows.Forms.FolderBrowserDialog();
                    if (System.Windows.Forms.DialogResult.OK.Equals(dialog.ShowDialog()))
                    {
                        path = dialog.SelectedPath;
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

            Application.Current.Dispatcher.Invoke(() =>
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

        public Task Refresh()
        {
            var tcs = new TaskCompletionSource<bool>();

            Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    var colorsDict = new ResourceDictionary { Source = new System.Uri(@"Styles/Colors.xaml", System.UriKind.Relative) };
                    var fontsDict = new ResourceDictionary { Source = new System.Uri(@"Styles/Fonts.xaml", System.UriKind.Relative) };
                    var textsDict = new ResourceDictionary { Source = new System.Uri(@"Styles/Texts.xaml", System.UriKind.Relative) };
                    var buttonsDict = new ResourceDictionary { Source = new System.Uri(@"Styles/Buttons.xaml", System.UriKind.Relative) };
                    var imagesDict = new ResourceDictionary { Source = new System.Uri(@"Styles/Images.xaml", System.UriKind.Relative) };
                    var collectionsDict = new ResourceDictionary { Source = new System.Uri(@"Styles/Collections.xaml", System.UriKind.Relative) };

                    Application.Current.Resources.MergedDictionaries.Clear();
                    Application.Current.Resources.MergedDictionaries.Add(colorsDict);
                    Application.Current.Resources.MergedDictionaries.Add(fontsDict);
                    Application.Current.Resources.MergedDictionaries.Add(textsDict);
                    Application.Current.Resources.MergedDictionaries.Add(buttonsDict);
                    Application.Current.Resources.MergedDictionaries.Add(imagesDict);
                    Application.Current.Resources.MergedDictionaries.Add(collectionsDict);
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
