using System.ComponentModel;

namespace Record.Recorder.Core
{
    /// <summary>
    /// A base view model that fires Property Changed events 
    /// </summary>
    public class BaseViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// The event that is fired when any child property changes its value
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        public void OnPropertyChanged(string name)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        protected void SetCurrentPageTo(ApplicationPage page)
        {
            IoC.Get<ApplicationViewModel>().CurrentPage = page;
        }
    }
}
