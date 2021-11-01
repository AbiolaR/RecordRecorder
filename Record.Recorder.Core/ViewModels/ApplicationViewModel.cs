namespace Record.Recorder.Core
{
    /// <summary>
    /// The application state as a view model
    /// </summary>
    public class ApplicationViewModel : BaseViewModel
    {
        /// <summary>
        /// The current page of the application
        /// </summary>
        public ApplicationPage CurrentPage { get; set; } = ApplicationPage.MainPage;
        public bool IsFocused { get; set; } = true;
        public bool IsRecordingInProgress { get; set; } = false;
        
        public ApplicationColor ApplicationForegroundColor { get; set; } = ApplicationColor.ForegroundLight;
        public ApplicationColor ApplicationBackgroundColor { get; set; } = ApplicationColor.BackgroundLight;
        public ApplicationColor ApplicationTextColor { get; set; } = ApplicationColor.TextDark;
        public ApplicationColor ApplicationShadowColor { get; set; } = ApplicationColor.ShadowLight;

        public ApplicationImage HomeIcon { get; set; } = ApplicationImage.HomeDark;
        public ApplicationImage GearIcon { get; set; } = ApplicationImage.GearDark;
    }
}
