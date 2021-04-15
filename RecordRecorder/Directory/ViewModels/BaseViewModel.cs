using PropertyChanged;
using System.ComponentModel;

namespace RecordRecorder
{

    class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
