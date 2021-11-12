namespace Record.Recorder.Core
{
    /// <summary>
    /// A base view model interface for any dialogs 
    /// </summary>
    public interface IBaseDialogViewModel
    {

        string Title { get; set; }
        DialogAnswer Answer { get; set; }
        //void OnDialogOpen();

    }
}
