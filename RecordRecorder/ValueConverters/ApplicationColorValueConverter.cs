using Record.Recorder.Core;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;

namespace RecordRecorder
{
    /// <summary>
    /// Converts the <see cref="ApplicationColor"/> to an actual view page
    /// </summary>
    class ApplicationColorValueConverter : BaseValueConverter<ApplicationColorValueConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((ApplicationColor)value)
            {
                case ApplicationColor.ForegroundDark:
                    return Application.Current.Resources["ForegroundDark"];

                case ApplicationColor.ForegroundLight:
                    return Application.Current.Resources["ForegroundLight"];

                case ApplicationColor.BackgroundDark:
                    return Application.Current.Resources["BackgroundDark"];

                case ApplicationColor.BackgroundLight:
                    return Application.Current.Resources["BackgroundLight"];

                case ApplicationColor.TextDark:
                    return Application.Current.Resources["TextDark"];

                case ApplicationColor.TextLight:
                    return Application.Current.Resources["TextLight"];

                case ApplicationColor.ShadowDark:
                    return Application.Current.Resources["ShadowDark"];

                case ApplicationColor.ShadowLight:
                    return Application.Current.Resources["ShadowLight"];


                default:
                    Debugger.Break();
                    return null;
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
