namespace Record.Recorder.Core
{
    public interface ISettingsManager
    {
        string GetRecordingDeviceName();
        void SetRecordingDeviceName(string value);
        
        string GetOutputFolderLocation();
        void SetOutputFolderLocation(string value);
        
        string GetAlbumName();
        void SetAlbumName(string value);

        string GetFileType();
        void SetFileType(string value);

        string GetSongDetectionType();
        void SetSongDetectionType(string value);
    }
}
