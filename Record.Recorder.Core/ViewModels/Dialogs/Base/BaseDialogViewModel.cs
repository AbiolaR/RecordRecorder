using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Record.Recorder.Core
{
    /// <summary>
    /// A base view model for any dialogs 
    /// </summary>
    public class BaseDialogViewModel : BaseViewModel
    {        
        public string Title { get; set; }
        public DialogAnswer Answer { get; set; } = DialogAnswer.None;
        public virtual void OnDialogOpen() { }
    }
}
