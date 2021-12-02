using System.Threading.Tasks;

namespace Record.Recorder.Core
{
    public interface IUpdateManager
    {
        Task DownloadTask { get; set; }
        Task<bool> CheckForUpdateAvailableAsync();

        string DownloadUpdate();

        void UpdateApplication();
    }
}
