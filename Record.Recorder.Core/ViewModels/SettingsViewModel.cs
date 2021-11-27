using Record.Recorder.Type;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
        public ICommand SetFileTypeToMp3Command { get; set; }
        public ICommand SetFileTypeToFlacCommand { get; set; }
        public ICommand SaveLanguageCommand { get; set; }

        protected readonly RecorderUtil recorder = new RecorderUtil();

        private KeyValuePair<int, string> noRecordingDevice = new KeyValuePair<int, string>(-2, Text.DeviceNameWatermark);
        private KeyValuePair<int, string> recordingDevice;
        private SortedDictionary<int, string> recordingDevices = new SortedDictionary<int, string>();
        private string outputFolderLocation, unsavedInputVolume;
        private bool isTestingRecordingDevice = false;
        private bool isThemeDark, isThemeLight, isFileTypeMp3, isFileTypeFlac;

        private List<string> languages = new List<string> {"English", "Deutsch"};
        public List<string> Languages { get => languages; set { languages = value; OnPropertyChanged(nameof(Languages)); } }
        private string currentLanguage;
        public string CurrentLanguage { get => currentLanguage; set { currentLanguage = value; OnPropertyChanged(nameof(CurrentLanguage)); } }

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
        public string UnsavedInputVolume { get => unsavedInputVolume; set { unsavedInputVolume = value; OnPropertyChanged(nameof(UnsavedInputVolume)); } }
        public bool IsTestingRecordingDevice { get => isTestingRecordingDevice; set { isTestingRecordingDevice = value; OnPropertyChanged(nameof(IsTestingRecordingDevice)); } }
        public bool IsThemeDark
        {
            get => isThemeDark;
            set
            {                
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
                isThemeLight = value;
                isThemeDark = !value;
                OnPropertyChanged(nameof(IsThemeLight));
                OnPropertyChanged(nameof(IsThemeDark));
            }
        }
        public bool IsFileTypeMp3
        {
            get => isFileTypeMp3;
            set
            {
                isFileTypeMp3 = value;
                isFileTypeFlac = !value;
                OnPropertyChanged(nameof(IsFileTypeMp3));
                OnPropertyChanged(nameof(IsFileTypeFlac));
            }
        }
        public bool IsFileTypeFlac
        {
            get => isFileTypeFlac;
            set
            {
                isFileTypeFlac = value;
                isFileTypeMp3 = !value;
                OnPropertyChanged(nameof(IsFileTypeFlac));
                OnPropertyChanged(nameof(IsFileTypeMp3));
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
            TestRecordingDeviceCommand = new RelayCommand((o) => TestRecordingDevice());
            SetThemeToLightCommand = new RelayCommand((o) => IsThemeLight = SetThemeToLight());
            SetThemeToDarkCommand = new RelayCommand((o) => IsThemeDark = SetThemeToDark());
            SetFileTypeToMp3Command = new RelayCommand((o) => IsFileTypeMp3 = SetFileTypeToMp3());
            SetFileTypeToFlacCommand = new RelayCommand((o) => IsFileTypeFlac = SetFileTypeToFlac());
            SaveLanguageCommand = new RelayCommand((o) => SaveLanguage(o));

            LoadSettingsData();
        }

        private void NavigateToMainPage()
        {
            recorder.StopRecording();
            SetCurrentPageTo(ApplicationPage.MainPage);
        }

        private bool SetFileTypeToMp3()
        {
            IoC.Settings.SaveFileType = AudioFileType.MP3;
            return true;
        }

        private bool SetFileTypeToFlac()
        {
            IoC.Settings.SaveFileType = AudioFileType.FLAC;
            return true;
        }


        private async void TestRecordingDevice()
        {
            if (await CheckForRecordingDevice())
            {
                ToggleCommmand(() => IsTestingRecordingDevice, () => { recorder.StopRecording(); }, () => { recorder.PlayRecordingDevice(); });
            }
            else
            {
                ShowPleaseChooseDeviceDialog();
            }
        }

        private async Task<bool> CheckForRecordingDevice()
        {
            string recordingDeviceName = IoC.Settings.RecordingDeviceName;

            if (string.IsNullOrEmpty(recordingDeviceName))
            {
                return false;
            }

            var recordingDevice = await recorder.GetRecordingDeviceByName(recordingDeviceName);
            if (recordingDevice.Equals(default(KeyValuePair<int, string>)))
            {
                return false;
            }
            return true;
        }
        async private void LoadSettingsData()
        {
            switch(IoC.Settings.ApplicationTheme)
            {
                case ApplicationTheme.DARK:
                    IsThemeDark = true;
                    break;

                default:
                    IsThemeLight = true;
                    break;
            }
            switch (IoC.Settings.SaveFileType)
            {
                case AudioFileType.FLAC:
                    IsFileTypeFlac = true;
                    break;

                default:
                    IsFileTypeMp3 = true;
                    break;
            }
            CurrentLanguage = IoC.Settings.ApplicationLanguage;
            OutputFolderLocation = IoC.Settings.OutputFolderLocation;
            RecordingDevices = await recorder.GetRecordingDevices();
            var recDevice = await recorder.GetRecordingDeviceByName(IoC.Settings.RecordingDeviceName);

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

        private void SaveLanguage(object o)
        {
            if (o != null)
            {               
                CurrentLanguage = (string)o;
                IoC.Settings.ApplicationLanguage = CurrentLanguage;
                switch (CurrentLanguage)
                {
                    case "Deutsch":
                        Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("de");
                        break;

                    default:
                        Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
                        break;
                }
                IoC.UI.Refresh();
                SetCurrentPageTo(ApplicationPage.MainPage);
                SetCurrentPageTo(ApplicationPage.SettingsPage);
            }
        }

        async private void UpdateRecordingDevices()
        {
            // stop playing recording device audio and reset boolean
            recorder.StopRecording();
            IsTestingRecordingDevice = false;

            RecordingDevices = await recorder.GetRecordingDevices();
            RecordingDevice = await recorder.GetRecordingDeviceByName(IoC.Settings.RecordingDeviceName);
        }

        async private void CheckIfSavedDeviceIsAvailable()
        {
            var recDevice = await recorder.GetRecordingDeviceByName(IoC.Settings.RecordingDeviceName);

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
                    IoC.Settings.RecordingDeviceName = keyValuePair.Value;
                    RecordingDevice = keyValuePair;
                }
            }
        }

        private async void SaveOutputFolderLocation()
        {
            var path = await IoC.UI.ChooseFolderLocation();
            if (!string.IsNullOrEmpty(path))
            {
                IoC.Settings.OutputFolderLocation = path;
                OutputFolderLocation = path;
            }
        }

        private async void OpenFolderLocation()
        {
            await IoC.UI.OpenFolderLocation(OutputFolderLocation);
        }

        #region Show Dialog Methods

        private async void ShowPleaseChooseDeviceDialog()
        {
            await IoC.UI.ShowMessage(new MessageBoxDialogViewModel
            {
                Title = Text.ChooseRecordingDevice,
                Message = Text.ChooseRecordingDeviceMessage,
                OkText = Text.OK
            });
        }

        #endregion
    }

}
