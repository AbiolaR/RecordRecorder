using Record.Recorder.Core;
using System.Diagnostics;
using System.Windows.Forms;

namespace RecordRecorder
{
    public class SettingsViewModel : BaseSettingsViewModel
    {

        protected override void ChooseFolderLocation()
        {
            using var dialog = new FolderBrowserDialog();
            if (DialogResult.OK.Equals(dialog.ShowDialog()))
            {
                SaveOutputFolderLocation(dialog.SelectedPath);
            }
        }

        protected override void OpenFolderLocation()
        {
            Process.Start("explorer.exe", OutputFolderLocation);
        }        
    }
}
