using Record.Recorder.Core;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Media.Imaging;

namespace RecordRecorder
{
    /// <summary>
    /// Converts the <see cref="ApplicationImage"/> to image source
    /// </summary>
    class ApplicationImageSourceConverter : BaseValueConverter<ApplicationImageSourceConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((ApplicationImage)value)
            {

                case ApplicationImage.HomeDark:
                    Application.Current.Resources["HomeIconSource"] = Application.Current.Resources["HomeIconDark"];
                    return ((BitmapImage)Application.Current.Resources["HomeIconDark"]).UriSource;

                case ApplicationImage.HomeLight:
                    Application.Current.Resources["HomeIconSource"] = Application.Current.Resources["HomeIconLight"];
                    return ((BitmapImage)Application.Current.Resources["HomeIconLight"]).UriSource;

                case ApplicationImage.GearDark:
                    Application.Current.Resources["GearIconSource"] = Application.Current.Resources["GearIconDark"];
                    return ((BitmapImage)Application.Current.Resources["GearIconDark"]).UriSource;

                case ApplicationImage.GearLight:
                    Application.Current.Resources["GearIconSource"] = Application.Current.Resources["GearIconLight"];
                    return ((BitmapImage)Application.Current.Resources["GearIconLight"]).UriSource;

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
