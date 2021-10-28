using System;
using System.Globalization;
using System.Windows.Media;

namespace RecordRecorder
{
    /// <summary>
    /// Converts the <see cref="SolidColorBrush"/> to a color
    /// </summary>
    class BurshToColorConverter : BaseValueConverter<BurshToColorConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((SolidColorBrush)value).Color;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
