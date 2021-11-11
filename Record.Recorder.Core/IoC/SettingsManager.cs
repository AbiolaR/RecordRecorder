using Record.Recorder.Type;
using System;

namespace Record.Recorder.Core
{
    public class SettingsManager : ISettingsManager
    {
        private static Properties.Settings Settings = Properties.Settings.Default;
        private static readonly string RECORDINGDEVICE = "RecordingDevice";
        private static readonly string APPLICATIONTHEME = "ApplicationTheme";
        private static readonly string OUTPUTFOLDERLOCATION = "OutputFolderLocation";
        private static readonly string ALBUMNAME = "AlbumName";
        private static readonly string SAVEFILETYPE = "SaveFileType";
        private static readonly string SONGDETECTIONTYPE = "SongDetectionType";


        public string RecordingDeviceName { get => Settings[RECORDINGDEVICE].ToString(); set => SetAndSave(RECORDINGDEVICE, value); }
        public string OutputFolderLocation { get => Settings[OUTPUTFOLDERLOCATION].ToString(); set => SetAndSave(OUTPUTFOLDERLOCATION, value); }
        public ApplicationTheme ApplicationTheme { get => (ApplicationTheme)Settings[APPLICATIONTHEME]; set => SetAndSave(APPLICATIONTHEME, value); }
        public string AlbumName { get => Settings[ALBUMNAME].ToString(); set => SetAndSave(ALBUMNAME, value); }
        public string SaveFileType { get => Settings[SAVEFILETYPE].ToString(); set => SetAndSave(SAVEFILETYPE, value); }
        public SongDetectionType SongDetectionType { get => (SongDetectionType)Settings[SONGDETECTIONTYPE]; set => SetAndSave(SONGDETECTIONTYPE, value); }

        private void SetAndSave(string setting, object value)
        {
            Settings[setting] = value;
            Settings.Save();
        }
    }
}
