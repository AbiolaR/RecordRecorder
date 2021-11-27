using System.Threading.Tasks;

namespace Record.Recorder.Core
{
    /// <summary>
    /// The UI manager that handles any UI interaction in the application
    /// </summary>
    public interface IUIManager
    {
        /// <summary>
        /// Displays a message box to the user
        /// </summary>
        /// <param name="viewModel">The view model</param>
        /// <returns></returns>
        Task ShowMessage(MessageBoxDialogViewModel viewModel);

        /// <summary>
        /// Displays a message box to the user with an additional button
        /// </summary>
        /// <param name="viewModel">The view model</param>
        /// <returns></returns>
        Task ShowMessageWithOption(MessageBoxButtonDialogViewModel viewModel);

        Task<string> ChooseFolderLocation();

        Task OpenFolderLocation(string path);

        Task ShowProgressDialogWithOption(ProgressBoxDialogViewModel viewModel);

        Task Refresh();
    }
}
