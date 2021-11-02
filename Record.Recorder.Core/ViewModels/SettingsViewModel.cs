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
        public ICommand UpdateDevicesCommand { get; set; }
        public ICommand CheckIfDeviceIsSelectedCommand { get; set; }
        public ICommand SaveFolderLocationCommand { get; set; }
        public ICommand SaveRecordingDeviceCommand { get; set; }
        public ICommand OpenFolderLocationCommand { get; set; }
        public ICommand UpdateInputVolumeCommand { get; set; }
        public ICommand TestRecordingDeviceCommand { get; set; }
        public ICommand SetThemeToLightCommand { get; set; }
        public ICommand SetThemeToDarkCommand { get; set; }

        private static readonly string DARK = "Dark";
        private static readonly string LIGHT = "Light";
        private static readonly string APPLICATIONTHEME = "ApplicationTheme";
        private static readonly string OUTPUTFOLDERLOCATION = "OutputFolderLocation";
        private static readonly string RECORDINGDEVICE = "RecordingDevice";

        protected readonly RecorderUtil recorder = new RecorderUtil();

        private KeyValuePair<int, string> noRecordingDevice = new KeyValuePair<int, string>(-2, Text.DeviceNameWatermark);
        private KeyValuePair<int, string> recordingDevice;
        private SortedDictionary<int, string> recordingDevices = new SortedDictionary<int, string>();
        private string outputFolderLocation, unsavedInputVolume; //inputVolume
        private bool isTestingRecordingDevice = false;
        private bool isThemeDark = false;
        private bool isThemeLight = true;

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

        public string OutputFolderLocation { get => outputFolderLocation; set { outputFolderLocation = value; OnPropertyChanged(nameof(OutputFolderLocation)); } }

        //public string InputVolume { get => inputVolume; set { inputVolume = value; OnPropertyChanged(nameof(InputVolume)); } }
        public string UnsavedInputVolume { get => unsavedInputVolume; set { unsavedInputVolume = value; OnPropertyChanged(nameof(UnsavedInputVolume)); } }
        public bool IsTestingRecordingDevice { get => isTestingRecordingDevice; set { isTestingRecordingDevice = value; OnPropertyChanged(nameof(IsTestingRecordingDevice)); } }
        public bool IsThemeDark
        {
            get => isThemeDark;
            set
            {
                Properties.Settings.Default[APPLICATIONTHEME] = DARK;
                Properties.Settings.Default.Save();
                isThemeDark = value;
                isThemeLight = !value;
                OnPropertyChanged(nameof(IsThemeDark));
                OnPropertyChanged(nameof(IsThemeLight));
            }
        }
        public bool IsThemeLight
        {
            get => isThemeLight;
            set
            {
                Properties.Settings.Default[APPLICATIONTHEME] = LIGHT;
                Properties.Settings.Default.Save();
                isThemeLight = value;
                isThemeDark = !value;
                OnPropertyChanged(nameof(IsThemeLight));
                OnPropertyChanged(nameof(IsThemeDark));
            }
        }


        public SettingsViewModel()
        {
            // Commands
            GoToHomeCommand = new RelayCommand((o) => NavigateToMainPage());
            UpdateDevicesCommand = new RelayCommand((o) => UpdateRecordingDevices());
            CheckIfDeviceIsSelectedCommand = new RelayCommand((o) => CheckIfSavedDeviceIsAvailable());
            SaveFolderLocationCommand = new RelayCommand((o) => SaveOutputFolderLocation());
            SaveRecordingDeviceCommand = new RelayCommand((o) => SaveRecordingDevice(o));
            OpenFolderLocationCommand = new RelayCommand((o) => OpenFolderLocation());
            //UpdateInputVolumeCommand = new RelayCommand((o) => UpdateInputVolume(o));
            TestRecordingDeviceCommand = new RelayCommand((o) => TestRecordingDevice());
            SetThemeToLightCommand = new RelayCommand((o) => IsThemeLight = SetThemeToLight());
            SetThemeToDarkCommand = new RelayCommand((o) => IsThemeDark = SetThemeToDark());

            LoadSettingsData();
        }

        private void NavigateToMainPage()
        {
            recorder.StopRecording();
            SetCurrentPageTo(ApplicationPage.MainPage);
        }


        private void TestRecordingDevice()
        {
            ToggleCommmand(() => IsTestingRecordingDevice, () => { recorder.StopRecording(); }, () => { recorder.PlayRecordingDevice(); });
        }

        /*private void UpdateInputVolume(object value)
        {
            if (value != null)
            {
                string formatedInputVolume = ((double)value).ToString("+0;-#");
                Properties.Settings.Default["inputVolume"] = formatedInputVolume;
                Properties.Settings.Default.Save();
                InputVolume = formatedInputVolume;
            }
        }*/

        async private void LoadSettingsData()
        {
            IsThemeDark = DARK.Equals(Properties.Settings.Default[APPLICATIONTHEME].ToString());
            OutputFolderLocation = Properties.Settings.Default[OUTPUTFOLDERLOCATION].ToString();
            //UnsavedInputVolume = InputVolume = Properties.Settings.Default["inputVolume"].ToString();
            RecordingDevices = await recorder.GetRecordingDevices();
            var recDevice = await recorder.GetRecordingDeviceByName(Properties.Settings.Default[RECORDINGDEVICE].ToString());

            if (recDevice.Equals(default(KeyValuePair<int, string>)))
            {
                RecordingDevices.Add(noRecordingDevice.Key, noRecordingDevice.Value);
                RecordingDevice = noRecordingDevice;
            }
            else
            {
                RecordingDevice = recDevice;
            }
        }



        async private void UpdateRecordingDevices()
        {
            // stop playing recording device audio and reset boolean
            recorder.StopRecording();
            IsTestingRecordingDevice = false;

            RecordingDevices = await recorder.GetRecordingDevices();
            RecordingDevice = await recorder.GetRecordingDeviceByName(Properties.Settings.Default[RECORDINGDEVICE].ToString());
        }

        async private void CheckIfSavedDeviceIsAvailable()
        {
            var recDevice = await recorder.GetRecordingDeviceByName(Properties.Settings.Default[RECORDINGDEVICE].ToString());

            if (recDevice.Equals(default(KeyValuePair<int, string>)))
            {
                RecordingDevices.Add(noRecordingDevice.Key, noRecordingDevice.Value);
                RecordingDevice = noRecordingDevice;
            }
        }

        private void SaveRecordingDevice(object o)
        {
            if (o != null)
            {
                var keyValuePair = (KeyValuePair<int, string>)o;
                if (keyValuePair.Key != -2)
                {
                    Properties.Settings.Default[RECORDINGDEVICE] = keyValuePair.Value;
                    Properties.Settings.Default.Save();
                    RecordingDevice = (keyValuePair);
                }
            }
        }

        private async void SaveOutputFolderLocation()
        {
            var path = await IoC.UI.ChooseFolderLocation();
            if (!string.IsNullOrEmpty(path))
            {
                Properties.Settings.Default[OUTPUTFOLDERLOCATION] = path;
                Properties.Settings.Default.Save();
                OutputFolderLocation = path;
            }
        }

        private async void OpenFolderLocation()
        {
            await IoC.UI.OpenFolderLocation(OutputFolderLocation);
        }

    }
}
