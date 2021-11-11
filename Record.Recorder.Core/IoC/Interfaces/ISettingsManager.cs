using Record.Recorder.Type;

namespace Record.Recorder.Core
{
    public interface ISettingsManager
    {
        string RecordingDeviceName { get; set; }
        string OutputFolderLocation { get; set; }
        ApplicationTheme ApplicationTheme { get; set; }
        string AlbumName { get; set; }
        string SaveFileType { get; set; }
        SongDetectionType SongDetectionType { get; set; }
    }
}
