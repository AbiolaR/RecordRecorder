using Record.Recorder.Core;
using System.Windows;
using System.Windows.Input;

namespace RecordRecorder
{
    /// <summary>
    /// The view model for the custom modern window 
    /// </summary>
    public class WindowViewModel : BaseViewModel
    {
        #region Private Properties
        /// <summary>
        /// The window this view model controls
        /// </summary>
        protected Window _window;

        private bool isDocked = false;

        /// <summary>
        /// The size of the resize border around the window
        /// </summary>
        private int resizeBorder { get { return Borderless ? 0 : 6; } }

        /// <summary>
        /// The margin around the window allowing for a drop shadow
        /// </summary>
        private int _outerMarginSize = 10;

        /// <summary>
        /// The radious of the window, creting curved corners
        /// </summary>
        private int _windowRadius = 10;
        #endregion

        #region Public Properties

        public double WindowMinimumWidth { get; set; } = 430;
        public double WindowMinimumHeight { get; set; } = 530;

        public bool Borderless { get { return (_window.WindowState == WindowState.Maximized || isDocked); } }

        /// <summary>
        /// The size of the resize border around the window, taking the outer margin into account
        /// </summary>
        public Thickness ResizeBorderThickness { get { return new Thickness(resizeBorder + OuterMarginSize); } }

        /// <summary>
        /// The margin around the window allowing for a drop shadow
        /// </summary>
        public int OuterMarginSize
        {
            get
            {
                // if window is maximized, return 0, removing the drop shadow margin, otherwise return it
                return Borderless ? 0 : _outerMarginSize;
            }
            set
            {
                _outerMarginSize = value;
            }
        }

        public Thickness OuterMarginThickness { get { return new Thickness(OuterMarginSize); } }

        public Thickness InnerContentPadding { get; set; } = new Thickness(0);

        /// <summary>
        /// The radious of the window, creting curved corners
        /// </summary>
        public int WindowRadius
        {
            get
            {
                // if window is maximized, return 0, removing the radius, otherwise return it
                return Borderless ? 0 : _windowRadius;
            }
            set
            {
                _windowRadius = value;
            }
        }

        public CornerRadius WindowCornerRadius { get { return new CornerRadius(WindowRadius); } }

        /// <summary>
        /// The height of the title bar of the window
        /// </summary>
        public int TitleHeight { get; set; } = 18;

        public GridLength TitleHeightGridLength { get { return new GridLength(TitleHeight + resizeBorder); } }



        /// <summary>
        /// The view model to use for the current page when the CurrentPage changes
        /// NOTE: This is not a live up-to-date view model of the current page
        ///       it is simply used to set the view model of the current page 
        ///       at the time it changes
        /// </summary>
        public BaseViewModel CurrentPageViewModel { get; set; }

        #endregion

        #region Commands

        /// <summary>
        /// The command to minimize the window
        /// </summary>
        public ICommand MinimizeCommand { get; set; }

        /// <summary>
        /// The command to maximize the window
        /// </summary>
        public ICommand MaximizeCommand { get; set; }

        /// <summary>
        /// The command to close the window
        /// </summary>
        public ICommand CloseCommand { get; set; }

        /// <summary>
        /// The command to show the system menu of the window
        /// </summary>
        public ICommand MenuCommand { get; set; }

        /// <summary>
        /// The command to switch to the settings page
        /// </summary>
        public ICommand SettingsViewCommand { get; set; }

        #endregion

        #region Public Helper Methods
        /*
        /// <summary>
        /// Navigates to the specified page
        /// </summary>
        /// <param name="page">The page to go to</param>
        /// <param name="viewModel">The view model, if any, to set explicitly to the new page</param>
        public void GoToPage(ApplicationPage page, BaseViewModel viewModel = null)
        {
            // Set the view model
            CurrentPageViewModel = viewModel;

            // See if page has changed
            //var different = CurrentPage != page;

            // Set the current page
            CurrentPage = page;

            // Fire off a CurrentPage changed event
            OnPropertyChanged(nameof(CurrentPage));

            // If the page hasn't changed, fire off notification
            // So pages still update if just the view model has changed
            //if (!different) { OnPropertyChanged(nameof(CurrentPage)); }
        }*/

        #endregion

        #region Constructor
        public WindowViewModel(Window window)
        {
            _window = window;


            // Listen for the window state changing
            _window.StateChanged += (sender, e) =>
            {
                WindowResized();
            };

            // Listen for the window size changing
            _window.SizeChanged += (sender, e) =>
            {
                isDocked = _window.WindowState == WindowState.Normal && _window.Width != _window.RestoreBounds.Width && _window.Height != _window.RestoreBounds.Height;
                WindowResized();
            };



            // Create commands
            MinimizeCommand = new RelayCommand((o) => _window.WindowState = WindowState.Minimized);
            MaximizeCommand = new RelayCommand((o) => _window.WindowState ^= WindowState.Maximized);
            CloseCommand = new RelayCommand((o) => window.Close());
            var mousePosition = Mouse.GetPosition(_window);
            MenuCommand = new RelayCommand((o) => SystemCommands.ShowSystemMenu(_window, new Point(mousePosition.X + _window.Left, mousePosition.Y + _window.Top)));



            var resizer = new WindowResizer(_window);

            // Listen out for dock changes
            /*
            resizer.WindowDockChanged += (dock) =>
            {
                // Store last position
                dockPosition = dock;

                // Fire off resize events
                WindowResized();
            };*/
        }
        #endregion


        #region Private Methods

        private void WindowResized()
        {
            // Fire off events for all properties that are affected by a resize
            OnPropertyChanged(nameof(ResizeBorderThickness));
            OnPropertyChanged(nameof(OuterMarginSize));
            OnPropertyChanged(nameof(OuterMarginThickness));
            OnPropertyChanged(nameof(WindowRadius));
            OnPropertyChanged(nameof(WindowCornerRadius));
        }

        /*protected async virtual void CloseWindow()
        {
            if(IoC.Get<ApplicationViewModel>().IsRecordingInProgress)
            {                
                await IoC.UI.ShowMessage(new MessageBoxDialogViewModel
                {
                    Title = "Recording in Progress",
                    Message = "You are currently recording.\nTo abort the recording click on Stop.",
                    OkText = "OK"
                });
            }
            else
            {
                try
                {
                    _window.Close();
                }
                catch { }
            }
        }*/

        #endregion
    }
}
