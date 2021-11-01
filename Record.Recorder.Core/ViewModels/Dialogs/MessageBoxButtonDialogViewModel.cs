using System.Windows.Input;

namespace Record.Recorder.Core
{
    public class MessageBoxButtonDialogViewModel : MessageBoxDialogViewModel
    {        
        public System.Action ButtonCommand { get; set; }
        public string ButtonText { get; set; }        

        public ICommand OptionCommand { get; set; }

        public MessageBoxButtonDialogViewModel()
        {
            OptionCommand = new RelayCommand((o) => ButtonCommand());
        }
    }
    
}
