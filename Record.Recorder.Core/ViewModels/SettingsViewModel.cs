using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Record.Recorder.Core
{
    public class SettingsViewModel : BaseViewModel
    {


        /// <summary>
        /// The command to switch to the main page
        /// </summary>
        public ICommand GoToHomeCommand { get; set; }
        public ICommand SelectedDeviceItemChanged { get; set; }
        public ICommand SetNewFolderLocationCommand { get; set; }
        public ICommand SaveRecordingDeviceCommand { get; set; }

        //public ICommand GoToHomeCommand { get; set; }

        public RecorderUtil recorder = new RecorderUtil();

        private KeyValuePair<int, string> recordingDevice;
        private SortedDictionary<int, string> recordingDevices = new SortedDictionary<int, string>();

        private string saveFolderLocation;

        public KeyValuePair<int, string> RecordingDevice
        {
            get => recordingDevice;
            set { recordingDevice = value; OnPropertyChanged(nameof(RecordingDevice)); }
        }

        public SortedDictionary<int, string> RecordingDevices
        {
            get => recordingDevices;
            set { recordingDevices = value; OnPropertyChanged(nameof(RecordingDevices)); }
        }

        public string SaveFolderLocation { get => saveFolderLocation; set => saveFolderLocation = value; }

        public SettingsViewModel()
        {
            // Now some navigational commands
            GoToHomeCommand = new RelayCommand((o) => SetCurrentPageTo(ApplicationPage.MainPage));
            SetNewFolderLocationCommand = new RelayCommand((o) => SetNewFolderLocation());
            SaveRecordingDeviceCommand = new RelayCommand((o) => SaveRecordingDevice(o));


            LoadSettingsData();
        }


        async private void LoadSettingsData()
        {
            SaveFolderLocation = Properties.Settings.Default["saveFolderLocation"].ToString();
            UpdateRecordingDevices();
            RecordingDevice = await recorder.GetRecordingDeviceByName(Properties.Settings.Default["recordingDevice"].ToString());

        }

        async private void UpdateRecordingDevices()
        {
            RecordingDevices = await recorder.GetRecordingDevices();
        }

        private void SaveRecordingDevice(object o)
        {
            if (o != null)
            {/*
                recordingDevices = recorder.GetRecordingDevices();
                OnPropertyChanged(nameof(RecordingDevices));
            } else
            {*/
                var valuePair = (KeyValuePair<int, string>)o;
                if (valuePair.Key != -2)
                {
                    Properties.Settings.Default["recordingDevice"] = valuePair.Value;
                    Properties.Settings.Default.Save();
                    RecordingDevice = (valuePair);
                }
            }
        }


        private void SetNewFolderLocation()
        {
            throw new NotImplementedException();
        }

    }
}
