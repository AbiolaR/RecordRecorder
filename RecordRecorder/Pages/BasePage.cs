using Record.Recorder.Core;
using System.Windows.Controls;

namespace RecordRecorder
{
    /// <summary>
    /// A true base page for all pages to gain base functionality
    /// </summary>
    public class BasePage<VM> : Page
        where VM : BaseViewModel, new()
    { 

        /// <summary>
        /// The View Model associated with this page
        /// </summary>
        private VM _viewModel;

        /// <summary>
        /// The View Model associated with this page
        /// </summary>
        public VM ViewModel
        {
            get { return _viewModel; }
            set
            {
                // Do nothing if nothing has changed
                if (_viewModel == value) { return; }

                // Update ViewModel value and set DataContext for this page
                _viewModel = value;
                DataContext = _viewModel;

            }
        }

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>  
        public BasePage()
        {
            // Create a default view model
            ViewModel = new VM();
        }

        #endregion
    }
}
