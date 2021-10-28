using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Threading.Tasks;

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

        public bool SetThemeToLight()
        {
            IoC.Get<ApplicationViewModel>().ApplicationBackgroundColor = ApplicationColor.BackgroundLight;
            IoC.Get<ApplicationViewModel>().ApplicationForegroundColor = ApplicationColor.ForegroundLight;
            IoC.Get<ApplicationViewModel>().ApplicationTextColor = ApplicationColor.TextDark;
            IoC.Get<ApplicationViewModel>().ApplicationShadowColor = ApplicationColor.ShadowLight;
            IoC.Get<ApplicationViewModel>().HomeIcon = ApplicationImage.HomeDark;
            IoC.Get<ApplicationViewModel>().GearIcon = ApplicationImage.GearDark;
            return true;
        }

        public bool SetThemeToDark()
        {
            IoC.Get<ApplicationViewModel>().ApplicationBackgroundColor = ApplicationColor.BackgroundDark;
            IoC.Get<ApplicationViewModel>().ApplicationForegroundColor = ApplicationColor.ForegroundDark;
            IoC.Get<ApplicationViewModel>().ApplicationTextColor = ApplicationColor.TextLight;
            IoC.Get<ApplicationViewModel>().ApplicationShadowColor = ApplicationColor.ShadowDark;
            IoC.Get<ApplicationViewModel>().HomeIcon = ApplicationImage.HomeLight;
            IoC.Get<ApplicationViewModel>().GearIcon = ApplicationImage.GearLight;
            return true;
        }

        /// <summary>
        /// Runs a command if the updating flag is not set.
        /// If the flag is true (indicating the function is already running) then the action is not run.
        /// If the flag is false (indicating the function is not running) then the action is run.
        /// Once the action is finished the flag is reset to false.
        /// </summary>
        /// <param name="updatingFlag">The boolean flag defining if the command is aleady running</param>
        /// <param name="action">The action to run if the command is not already running</param>
        /// <returns></returns>
        protected async Task RunCommand(Expression<Func<bool>> updatingFlag, Func<Task> action)
        {
            if (updatingFlag.GetPropertyValue())
                return;

            updatingFlag.SetPropertyValue(true);

            try
            {
                await action();
            }
            finally
            {
                updatingFlag.SetPropertyValue(false);
            }
        }

        protected void ToggleCommmand(Expression<Func<bool>> updatingFlag, System.Action trueAction, System.Action falseAction)
        {
            if (updatingFlag.GetPropertyValue())
            {
                try
                {
                    trueAction();
                }
                finally
                {
                    updatingFlag.SetPropertyValue(false);
                }
            }
            else
            {
                try
                {
                    falseAction();
                }
                finally
                {
                    updatingFlag.SetPropertyValue(true);
                }
            }
        }


    }
}
