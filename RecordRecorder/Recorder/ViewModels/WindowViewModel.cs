using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

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
        public int TitleHeight { get; set; } = 24;

        public GridLength TitleHeightGridLength { get { return new GridLength(TitleHeight + resizeBorder); } }

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
        }
        #endregion
    }
}
