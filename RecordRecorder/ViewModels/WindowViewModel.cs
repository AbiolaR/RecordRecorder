﻿using System.Windows;
using System.Windows.Input;

namespace RecordRecorder
{
    /// <summary>
    /// The view model for the custom modern window 
    /// </summary>
    class WindowViewModel : BaseViewModel
    {
        #region Private Properties
        /// <summary>
        /// The window this view model controls
        /// </summary>
        private Window _window;

        /// <summary>
        /// The size of the resize border around the window
        /// </summary>
        private int resizeBorder { get; set; } = 4;

        /// <summary>
        /// The margin around the window allowing for a drop shadow
        /// </summary>
        private int _outerMarginSize = 5;
        
        /// <summary>
        /// The radious of the window, creting curved corners
        /// </summary>
        private int _windowRadius = 10;
        #endregion

        #region Public Properties

        public double WindowMinimumWidth { get; set; } = 400;
        public double WindowMinimumHeight { get; set; } = 400;

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
                return _window.WindowState == WindowState.Maximized ? 0 : _outerMarginSize;
            }
            set
            {
                _outerMarginSize = value;
            }
        }

        public Thickness OuterMarginThickness { get { return new Thickness(OuterMarginSize); } }

        public Thickness InnerContentPadding { get { return new Thickness(resizeBorder);  } }

        /// <summary>
        /// The radious of the window, creting curved corners
        /// </summary>
        public int WindowRadius
        {
            get
            {
                // if window is maximized, return 0, removing the radius, otherwise return it
                return _window.WindowState == WindowState.Maximized ? 0 : _windowRadius;
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
        public int TitleHeight { get; set; } = 36;

        public GridLength TitleHeightGridLength { get { return new GridLength(TitleHeight + resizeBorder); } }

        public ApplicationPage CurrentPage { get; set; } = ApplicationPage.MainPage;

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

        #endregion

        #region Constructor
        public WindowViewModel(Window window)
        {
            _window = window;

            // Listen for the window resizing
            _window.StateChanged += (sender, e) =>
            {
                // Fire off events for all properties that are affected by a resize
                OnPropertyChanged(nameof(ResizeBorderThickness));
                OnPropertyChanged(nameof(OuterMarginSize));
                OnPropertyChanged(nameof(OuterMarginThickness));
                OnPropertyChanged(nameof(WindowRadius));
                OnPropertyChanged(nameof(WindowCornerRadius));
            };

            // Create commands
            MinimizeCommand = new RelayCommand((o) => _window.WindowState = WindowState.Minimized);
            MaximizeCommand = new RelayCommand((o) => _window.WindowState ^= WindowState.Maximized);
            CloseCommand = new RelayCommand((o) => window.Close());
            var mousePosition = Mouse.GetPosition(_window);
            MenuCommand = new RelayCommand((o) => SystemCommands.ShowSystemMenu(_window, new Point(mousePosition.X + _window.Left, mousePosition.Y + _window.Top)));

            var resizer = new WindowResizer(_window);
        }
        #endregion
    }
}