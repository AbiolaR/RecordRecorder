using Record.Recorder.Core;
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
            return Task.Run(() => MessageBox.Show(viewModel.Message, viewModel.Title));
        }
    }
}
