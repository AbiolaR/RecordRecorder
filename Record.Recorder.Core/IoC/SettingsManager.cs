namespace Record.Recorder.Core
{
    public class SettingsManager : ISettingsManager
    {
        private static Properties.Settings Settings = Properties.Settings.Default;
        private static readonly string RECORDINGDEVICE = "RecordingDevice";
        private static readonly string APPLICATIONTHEME = "ApplicationTheme";
        private static readonly string OUTPUTFOLDERLOCATION = "OutputFolderLocation";
        private static readonly string ALBUMNAME = "AlbumName";
        

        public string GetRecordingDeviceName()
        {
            return Settings[RECORDINGDEVICE].ToString();
        }

        public void SetRecordingDeviceName(string value)
        {
            Settings[RECORDINGDEVICE] = value;
            Settings.Save();
        }

        public string GetOutputFolderLocation()
        {
            return Settings[OUTPUTFOLDERLOCATION].ToString();
        }

        public void SetOutputFolderLocation(string value)
        {
            Settings[OUTPUTFOLDERLOCATION] = value;
            Settings.Save();
        }

        public string GetAlbumName()
        {
            return Settings[ALBUMNAME].ToString();
        }

        public void SetAlbumName(string value)
        {
            Settings[ALBUMNAME] = value;
            Settings.Save();
        }

        public string GetFileType()
        {
            throw new System.NotImplementedException();
        }

        public void SetFileType(string value)
        {
            throw new System.NotImplementedException();
        }

        public string GetSongDetectionType()
        {
            throw new System.NotImplementedException();
        }

        public void SetSongDetectionType(string value)
        {
            throw new System.NotImplementedException();
        }
    }
}
