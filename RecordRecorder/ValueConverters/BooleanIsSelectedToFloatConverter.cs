using System;
using System.Globalization;
using System.Windows;

namespace RecordRecorder
{
    /// <summary>
    /// Converts the <see cref="bool"/> to a float that is used for opacity
    /// </summary>
    class BooleanIsSelectedToFloatConverter : BaseValueConverter<BooleanIsSelectedToFloatConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? 0.7 : 1;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
